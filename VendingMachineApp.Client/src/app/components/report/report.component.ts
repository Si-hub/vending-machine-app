import { Component, OnInit } from '@angular/core';

import { ReportService } from 'src/app/services/report.service';
import { Purchase } from 'src/app/services/purchase.model';
import { Subject } from 'rxjs';
import * as XLSX from 'xlsx';
import { saveAs } from 'file-saver';
import { jsPDF } from 'jspdf';
import 'jspdf-autotable';

import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-report',
  templateUrl: './report.component.html',
  styleUrls: ['./report.component.css'],
})
export class ReportComponent implements OnInit {
  startDate: Date = new Date();
  endDate: Date = new Date();
  selectedDateRange: Date[] = [];
  selectedItems: Purchase[] = [];

  items: string[] = [
    "Sprite", 
    "Coke", 
    "Water", 
    "Oreo", 
    "Chips", 
    "Twist", 
    "Pepsi", 
    "Stoney", 
    "BarOne",
    'All items',
  ];

  dtOptions: DataTables.Settings = {};
  dtTrigger: Subject<any> = new Subject<any>();
  selectedItem: string = '';

  filteredItems: {
    purchaseId: number;
    itemId: number;
    itemName: string;
    amountPaid: number;
    change: number;
    purchaseDate: string;
  }[] = [];

  constructor(
    public reportService: ReportService,
    private toastr: ToastrService
  ) {}

  onChange(event: any) {
    console.log(event);
  }

  submitForm(): void {
    this.filterItems();
  }

  filterItems(): void {
    if (this.selectedDateRange && this.selectedItem) {
      const startDate = this.selectedDateRange[0];
      const endDate = this.selectedDateRange[1];

      const filteredItems = this.selectedItems.filter((item) => {
        const itemDate = new Date(item.purchaseDate);
        itemDate.setHours(0, 0, 0, 0); // Set the time to midnight

        return (
          (!startDate || itemDate >= startDate) &&
          (!endDate || itemDate <= endDate) &&
          (!this.selectedItem || item.itemName === this.selectedItem)
        );
      });

      this.filteredItems = filteredItems;
    }
    console.log(this.filteredItems);
  }

  updateFilteredItems(downloadedData: any[]): void {
    this.filteredItems = downloadedData;

    // Check if DataTable is already initialized
    if (this.filteredItems.length > 0) {
      this.dtTrigger.next(null); // Trigger DataTable update
    }
  }

  downloadPDF(): void {
    const data = this.filteredItems.map((item) => [
      item.purchaseId,
      item.itemName,
      this.formatCurrency(item.amountPaid),
      this.formatDate(item.purchaseDate),
    ]);

    const headers = [
      'Purchase ID',
      'Item Name',
      'Amount Paid',
      'Purchase Date',
    ];

    const doc = new jsPDF();

    // Add Report Title
    doc.text('Purchase History Report', 20, 10);

    // Add a line break
    doc.text('', 20, 30);

    (doc as any).autoTable({
      head: [headers],
      body: data,
    });

    doc.save('vending_machine_data.pdf');
    this.updateFilteredItems(data);
  }

  // Helper function to format currency
  formatCurrency(amount: number): string {
    return amount.toLocaleString('en-ZA', {
      style: 'currency',
      currency: 'ZAR',
    });
  }

  // Helper function to format date
  formatDate(date: string): string {
    const formattedDate = new Date(date);
    return formattedDate.toLocaleString('en-ZA', {
      day: 'numeric',
      month: 'long',
      year: 'numeric',
      hour: 'numeric',
      minute: 'numeric',
      second: 'numeric',
      hour12: true,
    });
  }

  downloadExcel(): void {
    const data = this.filteredItems.map((item) => {
      const amountPaid = typeof item.amountPaid === 'number' ? item.amountPaid : parseFloat((item.amountPaid as string).replace(/[^0-9.-]+/g, ''));
    
      if (!isNaN(amountPaid)) {
        const formattedAmountPaid = amountPaid.toLocaleString('en-ZA', {
          style: 'currency',
          currency: 'ZAR',
        });
    
        return {
          purchaseId: item.purchaseId,
          itemName: item.itemName,
          amountPaid: formattedAmountPaid,
          purchaseDate: new Date(item.purchaseDate).toLocaleDateString('en-ZA', {
            day: 'numeric',
            month: 'numeric',
            year: 'numeric',
            hour: 'numeric',
            minute: 'numeric',
          }),
        };
      } else {
        return {
          purchaseId: item.purchaseId,
          itemName: item.itemName,
          amountPaid: item.amountPaid.toString(), // Display non-numeric value as string
          purchaseDate: new Date(item.purchaseDate).toLocaleDateString('en-ZA', {
            day: 'numeric',
            month: 'numeric',
            year: 'numeric',
            hour: 'numeric',
            minute: 'numeric',
          }),
        };
      }
    });

    // Additional styling options
    const styleOptions = {
      header: {
        fill: { fgColor: { rgb: 'FF000000' } }, // Header background color (black)
        font: { color: { rgb: 'FFFFFFFF' }, bold: true }, // Header font color (white)
      },
      rows: {
        font: { size: 12, bold: false },
        fill: { fgColor: { rgb: 'FFD3D3D3' } }, // Row background color (light gray)
      },
    };

    // Create worksheet
    const worksheet = XLSX.utils.json_to_sheet(data);

    // Add headers to the worksheet
    worksheet['A1'] = { t: 's', v: 'Purchase ID' };
    worksheet['B1'] = { t: 's', v: 'Item Name' };
    worksheet['C1'] = { t: 's', v: 'Amount Paid' };
    worksheet['D1'] = { t: 's', v: 'Purchase Date' };

    // Apply header styling
    worksheet['A1'].s = styleOptions.header;
    worksheet['B1'].s = styleOptions.header;
    worksheet['C1'].s = styleOptions.header;
    worksheet['D1'].s = styleOptions.header;

    const ref = worksheet['!ref'];

    if (ref) {
      const range = XLSX.utils.decode_range(ref);

      for (let row = range.s.r + 1; row <= range.e.r; row++) {
        for (let col = range.s.c; col <= range.e.c; col++) {
          const cellRef = XLSX.utils.encode_cell({ r: row, c: col });
          const cell = worksheet[cellRef];

          // Apply row styling
          if (cell) {
            cell.s = styleOptions.rows;
          }
        }
      }
    } else {
      console.error(
        'Worksheet reference is undefined. Unable to apply row styling.'
      );
    }

    console.log('Header Style:', styleOptions.header);
    console.log('Row Style:', styleOptions.rows);

    // Generate Excel buffer
    const excelBuffer = XLSX.write(
      { Sheets: { data: worksheet }, SheetNames: ['data'] },
      { bookType: 'xlsx', type: 'array' }
    );

    // Save the Excel file
    const dataBlob = new Blob([excelBuffer], {
      type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
    });
    saveAs(dataBlob, 'vending_machine_data.xlsx');

    // Update filtered items if needed
    this.updateFilteredItems(data);
  }

  showPurchases(): void {
    this.reportService.getPurchaseData().subscribe({
      next: (res) => {
        this.selectedItems = res as Purchase[];
        this.filteredItems = res;
        this.dtTrigger.next(null);
      },
      error: (err) => {
        console.log(err);
      },
    });
  }

  onDelete(id: number) {
    if (confirm('Are you sure to delete this purchase?'))
      this.reportService.deletePurchaseDetail(id).subscribe({
        next: (res) => {
          this.reportService.list = res as Purchase[];
          this.toastr.error('Deleted successfully', 'Purchase Detail Record');
        },
        error: (err) => {
          console.log(err);
        },
      });
  }

  // Method to refresh or clear selected items and date range
  /*refresh(): void {
    // Clear selected items in the dropdown
    this.selectedItems = [];
    // Clear selected date range in the date picker
    this.selectedDateRange = [];
  }*/

  ngOnInit(): void {
    //datatables settings
    this.dtOptions = {
      pagingType: 'full_numbers',
      pageLength: 25,
      lengthMenu: [10, 25, 50],
      processing: true,
    };
    this.showPurchases();
  }
}

import { Component, OnInit, ViewChild } from '@angular/core';
import { DataTableDirective } from 'angular-datatables';

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

  @ViewChild(DataTableDirective, { static: false })
  dtElement: DataTableDirective | undefined;

  startDate: Date = new Date();
  endDate: Date = new Date();
  selectedDateRange: Date[] = [];
  selectedItems: Purchase[] = [];

  items: string[] = [
    'Sprite',
    'Coke',
    'Water',
    'Oreo',
    'Chips',
    'Twist',
    'Pepsi',
    'Stoney',
    'BarOne',
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
    if (this.selectedDateRange || this.selectedItem) {
      const startDate = this.selectedDateRange
        ? new Date(this.selectedDateRange[0])
        : null;
      const endDate = this.selectedDateRange
        ? new Date(this.selectedDateRange[1])
        : null;

      // Adjust start and end dates to cover the entire day
      if (startDate) {
        startDate.setHours(0, 0, 0, 0);
      }
      if (endDate) {
        endDate.setHours(23, 59, 59, 999);
      }

      this.filteredItems = this.selectedItems.filter((item) => {
        const itemDate = new Date(item.purchaseDate);

        const isWithinDateRange =
          (!startDate || itemDate.getTime() >= startDate.getTime()) &&
          (!endDate || itemDate.getTime() <= endDate.getTime());

          const matchesSelectedItem =
          !this.selectedItem || (this.selectedItem === 'All items') || item.itemName === this.selectedItem;

        return isWithinDateRange && matchesSelectedItem;
      });
    } else if (this.selectedItem) {
      // If only item is selected, filter by item
      this.filteredItems = this.selectedItems.filter(
        (item) => item.itemName === this.selectedItem
      );
    } else {
      // If no filters are set, show all items
      this.filteredItems = this.selectedItems;
    }
  }

  updateFilteredItems(downloadedData: any[]): void {
    this.filteredItems = downloadedData;

    // Check if DataTable is already initialized
    if (this.dtTrigger.observers.length === 0) {
      this.dtTrigger.next(null);
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
    // Update filtered items only if DataTable is not initialized
    if (!this.dtElement) {
      this.updateFilteredItems(data);
    }
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
      const amountPaid =
        typeof item.amountPaid === 'number'
          ? item.amountPaid
          : parseFloat((item.amountPaid as string).replace(/[^0-9.-]+/g, ''));

      if (!isNaN(amountPaid)) {
        const formattedAmountPaid = amountPaid.toLocaleString('en-ZA', {
          style: 'currency',
          currency: 'ZAR',
        });

        return {
          purchaseId: item.purchaseId,
          itemName: item.itemName,
          amountPaid: formattedAmountPaid,
          purchaseDate: new Date(item.purchaseDate).toLocaleString('en-ZA', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit',
            hour12: true,
          }),
        };
      } else {
        return {
          purchaseId: item.purchaseId,
          itemName: item.itemName,
          amountPaid: item.amountPaid.toString(), // Display non-numeric value as string
          purchaseDate: new Date(item.purchaseDate).toLocaleString('en-ZA', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit',
            hour12: true,
          }),
        };
      }
    });

    // Create worksheet
  const worksheet = XLSX.utils.json_to_sheet(data);

  // Add headers to the worksheet
  worksheet['A1'] = { t: 's', v: 'Purchase ID' };
  worksheet['B1'] = { t: 's', v: 'Item Name' };
  worksheet['C1'] = { t: 's', v: 'Amount Paid' };
  worksheet['D1'] = { t: 's', v: 'Purchase Date' };

  // Apply header styling
  worksheet['A1'].s = { font: { bold: true } };
  worksheet['B1'].s = { font: { bold: true } };
  worksheet['C1'].s = { font: { bold: true } };
  worksheet['D1'].s = { font: { bold: true } };

  // Apply row styling
  for (let row = 2; row <= data.length + 1; row++) {
    worksheet[`A${row}`].s = { fill: { fgColor: { rgb: 'FFD3D3D3' } } };
    worksheet[`B${row}`].s = { fill: { fgColor: { rgb: 'FFD3D3D3' } } };
    worksheet[`C${row}`].s = { fill: { fgColor: { rgb: 'FFD3D3D3' } } };
    worksheet[`D${row}`].s = { fill: { fgColor: { rgb: 'FFD3D3D3' } } };
  }

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

  // Update filtered items only if DataTable is not initialized
  if (!this.dtElement) {
    this.updateFilteredItems(data);
  }
   
  }

  showPurchases(): void {
    this.reportService.getPurchaseData().subscribe({
      next: (res) => {
        this.selectedItems = res as Purchase[];
        this.filteredItems = res;
        this.dtTrigger!.next(null); 
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
  resetForm() {
    this.selectedItem = ''; // Clear the selected item
    this.selectedDateRange = []; // Clear the selected date range
  }

  ngOnInit(): void {
    this.dtOptions = {
      pagingType: 'full_numbers',
      pageLength: 25,
      lengthMenu: [10, 25, 50],
      processing: true,
      searching: true,
      ordering: true,
      responsive: true,
    };
  
    this.showPurchases();
  }
}

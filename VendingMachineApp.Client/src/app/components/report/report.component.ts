import { Component, OnInit } from '@angular/core';
import { ReportService } from 'src/app/services/report.service';
import { Purchase } from 'src/app/services/purchase.model';
import { Subject } from 'rxjs';
import { saveAs } from 'file-saver';
import * as XLSX from 'xlsx';
import { jsPDF } from 'jspdf';
import { Items } from 'src/app/services/items.model';

@Component({
  selector: 'app-report',
  templateUrl: './report.component.html',
  styleUrls: ['./report.component.css'],
})
export class ReportComponent implements OnInit {
  selectedDateRange: Date[] = [];
  selectedItems: Purchase[] = [];
  items: string[] = [
    'Sprite',
    'Pepsi',
    'Lemonade',
    'Coke',
    'Water',
    'Root Beer',
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

  constructor(public reportService: ReportService) {}

  onChange(event: any) {}

  submitForm(): void {
    this.filterItems();
  }

  filterItems(): void {
    if (this.selectedDateRange && this.selectedItem) {
      const startDate = this.selectedDateRange[0];
      const endDate = this.selectedDateRange[1];

      const formattedStartDate = startDate
        ? startDate.toISOString().split('T')[0]
        : null;
      const formattedEndDate = endDate
        ? endDate.toISOString().split('T')[0]
        : null;

      const filteredItems = this.selectedItems.filter((item) => {
        return (
          (!formattedStartDate || item.purchaseDate >= formattedStartDate) &&
          (!formattedEndDate || item.purchaseDate <= formattedEndDate) &&
          (!this.selectedItem || item.itemName === this.selectedItem)
        );
      });

      this.selectedItems = filteredItems;
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
      item.amountPaid,
      item.purchaseDate,
    ]);
    const headers = [
      'Purchase ID',
      'Item Name',
      'Amount Paid',
      'Purchase Date',
    ];
    const doc = new jsPDF();
    (doc as any).autoTable({
      head: [headers],
      body: data,
    });
    doc.save('vending_machine_data.pdf');

    this.updateFilteredItems(data);
  }

  downloadExcel(): void {
    const data = this.filteredItems.map((item) => ({
      purchaseId: item.purchaseId,
      itemName: item.itemName,
      amountPaid: item.amountPaid,
      purchaseDate: item.purchaseDate,
    }));
    const worksheet = XLSX.utils.json_to_sheet(data);
    const workbook = { Sheets: { data: worksheet }, SheetNames: ['data'] };
    const excelBuffer = XLSX.write(workbook, {
      bookType: 'xlsx',
      type: 'array',
    });
    const dataBlob = new Blob([excelBuffer], {
      type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
    });
    saveAs(dataBlob, 'vending_machine_data.xlsx');

    this.updateFilteredItems(data);
  }

  showPurchases(): void {
    this.reportService.getPurchaseData().subscribe({
      next: (res) => {
        this.selectedItems = res as Purchase[];
        this.dtTrigger.next(null);
      },
      error: (err) => {
        console.log(err);
      },
    });
  }

  ngOnInit(): void {
    this.dtOptions = {
      pagingType: 'full_numbers',
      pageLength: 5,
      lengthMenu: [5, 10, 25],
      processing: true,
    };
    this.showPurchases();
  }
}

import { Component, OnInit } from '@angular/core';

import { ReportService } from 'src/app/services/report.service';
import { Purchase } from 'src/app/services/purchase.model';
import { Subject } from 'rxjs';
import { saveAs } from 'file-saver';
import * as XLSX from 'xlsx';
import { jsPDF } from 'jspdf';
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

  constructor(public reportService: ReportService, private toastr: ToastrService) {}

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

      // Set the end date to the end of the day
    if (endDate) {
      endDate.setHours(23, 59, 59, 999);
    }

      const formattedStartDate = startDate
        ? startDate.toISOString().split('T')[0]
        : null;
      const formattedEndDate = endDate
        ? endDate.toISOString().split('T')[0]
        : null;

      // Filter by date range and product
      const filteredItems = this.selectedItems.filter((item) => {
        return (
          (!formattedStartDate || item.purchaseDate >= formattedStartDate) &&
          (!formattedEndDate || item.purchaseDate < formattedEndDate) &&
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


  generateReport(format: string) {
    this.reportService.generatePurchaseReport(this.startDate, this.endDate, format)
      .subscribe((data: Blob) => {
        this.downloadReport(data, format);
      });
  }

  downloadReport(data: Blob, format: string) {
    const blob = new Blob([data], { type: 'application/octet-stream' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `purchase_report.${format}`;
    a.click();
    window.URL.revokeObjectURL(url);
    a.remove();
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

  onDelete(id: number) {
    if (confirm('Are you sure to delete this purchase?'))
      this.reportService.deletePurchaseDetail(id)
        .subscribe({
          next: res => {
            this.reportService.list = res as Purchase[]
            this.toastr.error('Deleted successfully', 'Purchase Detail Record')
          },
          error: err => { console.log(err) }
        })
  }
  ngOnInit(): void {
    //datatables settings
    this.dtOptions = {
      pagingType: 'full_numbers',
      pageLength: 10,
      lengthMenu: [10, 25, 50],
      processing: true,
    };
    this.showPurchases();
  }
}

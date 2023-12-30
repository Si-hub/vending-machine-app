import { Component, OnInit } from '@angular/core';
import { VendingMachineService } from 'src/app/services/vending-machine.service';
import { FormBuilder } from '@angular/forms';
import { NgForm } from '@angular/forms';
import { Purchase } from 'src/app/services/purchase.model';
import { ToastrService } from 'ngx-toastr';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-vending-machine',
  templateUrl: './vending-machine.component.html',
  styleUrls: ['./vending-machine.component.css'],
})
export class VendingMachineComponent implements OnInit {
  items: any[] = [];
  SelectedItem = '';
  amountPaid: number = 0;
  change: number = 0;
  purchaseDate: string = '';

  changeAmount: number =0;
  showChangeAmount: boolean = false;

  constructor(
    private fb: FormBuilder,
    public vendingMachineService: VendingMachineService,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.getItems();
  }

  onItemSelect(item: any) {
    console.log(item);
    this.vendingMachineService.formData = new Purchase(
      0,
      item.itemId,
      item.itemName,
      this.amountPaid,
      this.change,
      this.purchaseDate
    );
  }

  getItems(): void {
    this.vendingMachineService.getItem().subscribe((data) => {
      console.log(data);
      this.items = data;
    });
  }

  onSubmit(form: NgForm) {
    this.vendingMachineService.formSubmitted = true;
    if (form.valid) {
      if (this.vendingMachineService.formData.purchaseId == 0)
        this.makePurchase(form);
    }
  }

  makePurchase(form: NgForm) {
    const itemId = this.vendingMachineService.formData.itemId;
    const amountPaid = this.vendingMachineService.formData.amountPaid;

    this.vendingMachineService.addPurchases(itemId, amountPaid).subscribe({
      next: (res) => {
        this.vendingMachineService.list = res as Purchase[];
        this.vendingMachineService.resetForm(form);
        this.toastr.success('Purchase successful 😊', 'Success');

        // Call my existing method to calculate change
        this.updateChangeAmount();

        // Manually trigger change detection
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Error making the purchase:', error);
        if (error.error && error.error.errorMessage) {
          // Display the out-of-stock error message
          this.toastr.error(error.error.errorMessage, 'Out of Stock');
        } else {
          // Display a generic error message
          this.toastr.error(
            'Error occurred while making the purchase. Please try again. 😞',
            'Error'
          );
        }
      },
    });
  }

  getCalculatedChange() {
    const amountPaid = this.vendingMachineService.formData.amountPaid;
    const selectedId = this.vendingMachineService.formData.itemId;
    const selectedItem = this.items.filter(
      (item: any) => item.itemId === selectedId
    );
    console.log(amountPaid);
    console.log(selectedId);
    console.log(selectedItem);

    if (selectedItem.length > 0) {
      var change = amountPaid - parseFloat(selectedItem[0].itemPrice);
      console.log(change);
      return change;
    } else {
      return 0;
    }
  }

  // Call this method after the Toastr success notification
  updateChangeAmount() {
    this.changeAmount = this.getCalculatedChange();
    this.showChangeAmount = true;
  }
  onCancelPurchase(form: NgForm) {
    // Logic to handle cancellation of the current purchase
    form.resetForm();
    // Display info toast notification
    this.toastr.info('Purchase cancelled 😢', 'Info');
  }
}

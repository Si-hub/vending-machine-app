import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { VendingMachineService } from 'src/app/services/vending-machine.service';
import { FormBuilder } from '@angular/forms';
import { NgForm } from '@angular/forms';
import { Purchase } from 'src/app/services/purchase.model';
import { ToastrService } from 'ngx-toastr';

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
  showCalculatedChange: boolean = false;

  constructor(
    private fb: FormBuilder,
    public vendingMachineService: VendingMachineService,
    private toastr: ToastrService,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.getItems();
  }

  onItemSelect(item: any) {
    console.log(item);
    // Preserve the existing amountPaid value if it's greater than 0
    const amountPaid = this.vendingMachineService.formData.amountPaid;
    this.vendingMachineService.formData = new Purchase(
      0,
      item.itemId,
      item.itemName,
      amountPaid > 0 ? amountPaid : 0, // Set amountPaid to 0 if it's not greater than 0
      0,
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
        this.vendingMachineService.formData.change = this.getCalculatedChange();

        // Set showCalculatedChange to true to display the calculated change
        this.showCalculatedChange = true;
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
      return amountPaid - parseFloat(selectedItem[0].itemPrice);
    } else {
      return 0;
    }
  }

  onCancelPurchase(form: NgForm) {
  this.vendingMachineService.resetForm(form);
  this.vendingMachineService.formData.change = 0;
  
    // Display info toast notification
    this.toastr.info('Purchase cancelled 😢', 'Info');
  }
}

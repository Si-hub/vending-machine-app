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
    console.log('Selected Item:', item);
    // Preserve the existing amountPaid value if it's greater than 0
    const amountPaid = this.vendingMachineService.formData.amountPaid;
  
    // Set the itemId in the formData
    this.vendingMachineService.formData = new Purchase(
      0,
      item.itemId,  // Make sure item.itemId is correct based on your dynamic data
      item.itemName,
      amountPaid > 0 ? amountPaid : 0,
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

        // Log the amountPaid to check if it's correct
        console.log('Amount Paid:', amountPaid);

        // Make sure to set the updated amountPaid after the purchase
        this.vendingMachineService.formData.amountPaid = amountPaid;

        // Call my existing method to calculate change
        this.vendingMachineService.formData.change = this.getCalculatedChange();

        // Log the calculated change to check its value
        console.log(
          'Calculated Change:',
          this.vendingMachineService.formData.change
        );

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
    let selectedId = this.vendingMachineService.formData.itemId;
  
    console.log('Amount Paid in getCalculatedChange:', amountPaid);
    console.log('Selected Id:', selectedId);
    console.log('Items Array:', this.items);
  
    if (!selectedId) {
      // If selectedId is 0 or falsy, set it to the first item's itemId
      selectedId = this.items[0].itemId;
      this.vendingMachineService.formData.itemId = selectedId; // Update the formData
    }
  
    const selectedItem = this.items.find((item) => item.itemId === selectedId);
  
    console.log('Selected Item:', selectedItem);
  
    if (selectedItem) {
      var change = amountPaid - parseFloat(selectedItem.itemPrice);
  
      // Ensure the calculated change is not negative
      change = Math.max(change, 0);
  
      console.log('Calculated Change:', change);
      return change;
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

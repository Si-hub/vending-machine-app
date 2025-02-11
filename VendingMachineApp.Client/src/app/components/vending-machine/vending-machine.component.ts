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
  SelectedItem: string | null = '';
  amountPaid: number = 0; // Ensure this variable exists
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
    
    // Set the selected item
    this.SelectedItem = item.itemId; 
  
    // Update formData with the selected item
    this.vendingMachineService.formData = new Purchase(
      0,
      item.itemId,
      item.itemName,
      this.vendingMachineService.formData.amountPaid || 0,
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
  
        // Reset the selected item
        this.SelectedItem = null; 
  
        // Call the method to calculate and display change
        this.vendingMachineService.formData.change = this.getCalculatedChange();
        this.showCalculatedChange = true;
      },
      error: (error) => {
        console.error('Error making the purchase:', error);
        if (error.error && error.error.errorMessage) {
          this.toastr.error(error.error.errorMessage, 'Out of Stock');
        } else {
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
    this.SelectedItem = null;  // Reset selected item
    this.showCalculatedChange = false; // Hide change display
  
    // Display info toast notification
    this.toastr.info('Purchase cancelled 😢', 'Info');
  }
  
}

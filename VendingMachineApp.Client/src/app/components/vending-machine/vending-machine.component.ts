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
  SelectedItem: string | null = null;
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
    this.SelectedItem = item.itemId;

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

  getSelectedItemPrice(): number {
    if (!this.SelectedItem) return 0; // If no item is selected, return 0
  
    const selectedItem = this.items.find((item) => item.itemId === this.SelectedItem);
    return selectedItem ? parseFloat(selectedItem.itemPrice) : 0;
  }
  

  /**
   * Validates the form before submission
   */
  validateForm(): boolean {
    if (!this.SelectedItem) {
      this.toastr.error('Please select an item before proceeding.', 'Error');
      return false;
    }

    if (!this.vendingMachineService.formData.amountPaid || this.vendingMachineService.formData.amountPaid < 5) {
      this.toastr.error('Minimum amount required is R5.00.', 'Error');
      return false;
    }

    const selectedItem = this.items.find((item) => item.itemId === this.SelectedItem);
    if (selectedItem && this.vendingMachineService.formData.amountPaid < selectedItem.itemPrice) {
      this.toastr.error(`Insufficient funds. Please insert at least R${selectedItem.itemPrice}.`, 'Error');
      return false;
    }

    return true;
  }

  onSubmit(form: NgForm) {
    this.vendingMachineService.formSubmitted = true;

    if (!this.validateForm()) {
      return;
    }

    if (this.vendingMachineService.formData.purchaseId == 0) {
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

        this.SelectedItem = null;
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

    if (!selectedId) {
      selectedId = this.items[0]?.itemId;
      this.vendingMachineService.formData.itemId = selectedId;
    }

    const selectedItem = this.items.find((item) => item.itemId === selectedId);

    if (selectedItem) {
      let change = amountPaid - parseFloat(selectedItem.itemPrice);
      return Math.max(change, 0);
    } else {
      return 0;
    }
  }

  onCancelPurchase(form: NgForm) {
    this.vendingMachineService.resetForm(form);
    this.vendingMachineService.formData.change = 0;
    this.SelectedItem = null;
    this.showCalculatedChange = false;
    this.toastr.info('Purchase cancelled 😢', 'Info');
  }
}

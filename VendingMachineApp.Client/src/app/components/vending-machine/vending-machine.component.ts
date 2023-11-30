import { Component, OnInit } from '@angular/core';
import { VendingMachineService } from 'src/app/services/vending-machine.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgForm } from '@angular/forms';
import { Purchase } from 'src/app/services/purchase.model';

@Component({
  selector: 'app-vending-machine',
  templateUrl: './vending-machine.component.html',
  styleUrls: ['./vending-machine.component.css'],
})
export class VendingMachineComponent implements OnInit {
  items: any[] = [];
  selectedItem = '';
  purchaseData: any;
  amountPaid: number = 0;
  change: number = 0;
  purchaseDate = '';

  constructor(
    private fb: FormBuilder,
    public vendingMachineService: VendingMachineService
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
    this.vendingMachineService.addPurchases().subscribe({
      next: (res) => {
        this.vendingMachineService.list = res as Purchase[];
        this.vendingMachineService.resetForm(form);
        //this.toastr.success('Inserted successfully', 'Payment Detail Register')
      },
      error: (err) => {
        console.log(err);
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
      return '';
    }
  }

  onCancelPurchase(form: NgForm) {
    // Reset the form 
    form.resetForm();
  }
}

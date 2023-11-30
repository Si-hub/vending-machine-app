import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Purchase } from 'src/app/services/purchase.model';
import { NgForm } from '@angular/forms';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Items } from './items.model';

@Injectable({
  providedIn: 'root',
})
export class VendingMachineService {
  // Define my API endpoint URL.
  private itemsApiUrl: string = environment.apiUrl + 'items';
  private purchaseApiUrl: string = environment.apiUrl + 'purchases';

  list: Purchase[] = [];
  formSubmitted: boolean = false;
  formData: Purchase = new Purchase(0, 0, '', 0, 0, '');
  

  constructor(private http: HttpClient) {}

  // Create a method to fetch items from the backend.
  getItem() {
    return this.http.get<any>(this.itemsApiUrl);
  }



  addPurchases(){
    return this.http.post(this.purchaseApiUrl, this.formData)
  }

  resetForm(form: NgForm) {
    form.form.reset()
    this.formData = new Purchase(0, 0, '', 0, 0, '')
    this.formSubmitted = false
  }
}

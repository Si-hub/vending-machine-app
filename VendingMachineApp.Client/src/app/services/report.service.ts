import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Purchase } from 'src/app/services/purchase.model';

@Injectable({
  providedIn: 'root',
})
export class ReportService {
  private apiUrl = environment.apiUrl + 'purchases';
  private reportsUrl = environment.apiUrl + 'reports/GeneratePurchaseReport';
  list: Purchase[] = [];

  constructor(private http: HttpClient) {}

  getPurchaseData() {
    return this.http.get<any[]>(this.apiUrl);
  }

  deletePurchaseDetail(id: number) {
    return this.http.delete(this.apiUrl + '/' + id)
  }

}

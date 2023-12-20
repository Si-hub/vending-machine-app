import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
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

  generatePurchaseReport(startDate: Date, endDate: Date, format: string): Observable<Blob> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    const options = { headers: headers, responseType: 'blob' as 'json' };
    const requestPayload = { startDate: startDate.toISOString(), endDate: endDate.toISOString(), format: format };

    return this.http.get<Blob>(this.reportsUrl, { ...options, params: requestPayload });
  }

  deletePurchaseDetail(id: number) {
    return this.http.delete(this.apiUrl + '/' + id)
  }

}

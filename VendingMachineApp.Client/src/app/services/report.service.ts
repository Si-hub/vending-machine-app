import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Purchase } from 'src/app/services/purchase.model';

@Injectable({
  providedIn: 'root',
})
export class ReportService {
  private apiUrl = environment.apiUrl + 'purchases';
  dataSource: Purchase[] = [];

  constructor(private http: HttpClient) {}

  getPurchaseData() {
    return this.http.get<any[]>(this.apiUrl);
  }
}

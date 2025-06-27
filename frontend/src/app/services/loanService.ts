import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Loan } from '../models/loan.model';

import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})

export class LoanService {
  private readonly apiUrl = `${environment.apiUrl}/loans`;

  constructor(private readonly http: HttpClient) {}

  getLoans(page: number, pageSize: number): Observable<Loan[]> {
    return this.http.get<Loan[]>(`${this.apiUrl}?page=${page}&pageSize=${pageSize}`);
  }
}
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { Loan } from './models/loan.model';
import { LoanService } from './services/loanService';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  loans: Loan[] = [];
  displayedColumns: string[] = [
    'clientCode',
    'clientName',
    'originalAmount',
    'outstandingBalance',
    'interestRate',
    'paymentAmount',
    'status',
  ];

  ngOnInit() {
    this.loadLoans();
  }

  constructor(private readonly loanService: LoanService) {
  }

  loadLoans(page = 1, pageSize = 5) {
    this.loanService.getLoans(page, pageSize).subscribe(loans => {
      this.loans = loans;
    });
  }
}

import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  displayedColumns: string[] = [
    'loanAmount',
    'currentBalance',
    'applicant',
    'status',
  ];
  loans = [
    {
      loanAmount: 5000,
      currentBalance: 2000,
      applicant: 'John Doe',
      status: 'Approved',
    },
    {
      loanAmount: 10000,
      currentBalance: 8000,
      applicant: 'Jane Smith',
      status: 'Pending',
    },
    {
      loanAmount: 15000,
      currentBalance: 12000,
      applicant: 'Alice Johnson',
      status: 'Rejected',
    },
  ];
}

import { Client } from "./client.model";

export interface Loan {
    client: Client
    code: string;
    loanType: string;
    status: string;
    originalAmount: number;
    outstandingBalance: number; 
    interestRate: number;
    paymentAmount: number;
    paymentDay: number;     
    paymentDueDate: string;
    lastPaymentDate: string;
    rowId: string;
}

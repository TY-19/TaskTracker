import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Employee } from "../models/employee";

@Injectable({
    providedIn: 'root',
  })
export class EmployeeService {
    
  constructor(private http: HttpClient) {

  }

  getAllEmployees(): Observable<Employee[]> {
    const url = "/api/employees";
    return this.http.get<Employee[]>(url);
  }

  getEmployees(boardId: string): Observable<Employee[]> {
    const url = "/api/boards/" + boardId + "/employees";
    return this.http.get<Employee[]>(url);
  }

  addEmployeeToTheBoard(boardId: string, userName: string) {
    const url = "/api/boards/" + boardId + "/employees/" + userName;
    return this.http.post(url, null);
  }

  removeEmployeeFromTheBoard(boardId: string, employeeId: string | number) {
    const url = "/api/boards/" + boardId + "/employees/" + employeeId;
    return this.http.delete(url);
  }
}
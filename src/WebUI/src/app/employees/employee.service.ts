import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Employee } from "../models/employee";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: 'root',
  })
export class EmployeeService {
    
  constructor(private http: HttpClient) {

  }

  getAllEmployees(): Observable<Employee[]> {
    const url = environment.baseUrl + "api/employees";
    return this.http.get<Employee[]>(url);
  }

  getEmployees(boardId: number | string): Observable<Employee[]> {
    const url = environment.baseUrl + "api/boards/" + boardId + "/employees";
    return this.http.get<Employee[]>(url);
  }

  getEmployee(boardId: number | string, employeeId: number | string): Observable<Employee> {
    const url = environment.baseUrl + "api/boards/" + boardId + "/employees/" + employeeId;
    return this.http.get<Employee>(url);
  }

  addEmployeeToTheBoard(boardId: string, userName: string) {
    const url = environment.baseUrl + "api/boards/" + boardId + "/employees/" + userName;
    return this.http.post(url, null);
  }

  removeEmployeeFromTheBoard(boardId: string, employeeId: string | number) {
    const url = environment.baseUrl + "api/boards/" + boardId + "/employees/" + employeeId;
    return this.http.delete(url);
  }
}
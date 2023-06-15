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

  getEmployees(boardId: string): Observable<Employee[]> {
    const url = "/api/boards/" + boardId + "/employees";
    return this.http.get<Employee[]>(url);
  }
}
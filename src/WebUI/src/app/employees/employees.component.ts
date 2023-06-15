import { Component, OnInit } from '@angular/core';
import { EmployeeService } from './employee.service';
import { ActivatedRoute } from '@angular/router';
import { Employee } from '../models/employee';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'tt-employees',
  templateUrl: './employees.component.html',
  styleUrls: ['./employees.component.scss']
})
export class EmployeesComponent implements OnInit {
  
  boardId: string = "";
  employees!: MatTableDataSource<Employee>;

  constructor(private employeeService: EmployeeService, 
    private activatedRoute: ActivatedRoute) { 

  }

  ngOnInit(): void {
    this.boardId = this.activatedRoute.snapshot.paramMap.get('boardId')!;
    this.getEmployees();
  }

  getEmployees() {
    this.employeeService.getEmployees(this.boardId)
      .subscribe(result => 
        {
          console.log(result);
          this.employees = new MatTableDataSource(result);
        }
        );
    console.log(this.employees);
  }

}

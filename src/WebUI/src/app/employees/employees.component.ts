import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { EmployeeService } from './employee.service';
import { ActivatedRoute } from '@angular/router';
import { Employee } from '../models/employee';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { RolesService } from '../users/roles.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableHelper } from '../common/helpers/mat-table-helper';

@Component({
  selector: 'tt-employees',
  templateUrl: './employees.component.html',
  styleUrls: ['./employees.component.scss']
})
export class EmployeesComponent implements OnInit {
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild('filter') filter!: ElementRef;

  tableHelper = new MatTableHelper<Employee>();
  
  boardId: string = "";
  boardEmployees: Employee[] = [];
  employeesTable!: MatTableDataSource<Employee>;

  showPanel: boolean = false;
  mode: string = "view";

  constructor(private employeeService: EmployeeService,
    public rolesService: RolesService,
    private activatedRoute: ActivatedRoute) { 

  }

  ngOnInit(): void {
    this.boardId = this.activatedRoute.snapshot.paramMap.get('boardId')!;
    this.getEmployees();
  }

  ngAfterViewInit() {
    this.tableHelper.initiateTable(this.employeesTable, this.sort, this.paginator);
  }

  getEmployees() {
    this.employeeService.getEmployees(this.boardId)
      .subscribe(result => {
        this.boardEmployees = result;
        this.employeesTable = new MatTableDataSource<Employee>(result);
        this.tableHelper.initiateTable(this.employeesTable, this.sort, this.paginator);
      });
  }
  
  changeMode(mode: string) {
    this.mode = mode;
    if (mode === 'create' || mode === 'edit')    
        this.showPanel = true;
    else
        this.showPanel = false;
  }

  onFilterTextChanged(filterText: string) {
    this.employeesTable.data = this.boardEmployees
      .filter(x => x.userName.toLowerCase().includes(filterText.toLowerCase()) 
          || (x.lastName?.includes(filterText.toLowerCase()) ?? false)
          || (x.firstName?.includes(filterText.toLowerCase()) ?? false)
          || (x.roles.filter(r => r.toLowerCase().includes(filterText.toLowerCase())).length > 0));
  }

  clearFilter() {
    this.filter.nativeElement['value'] = '';
    this.onFilterTextChanged('');
  }

  onAddingEmployee(employee: Employee) {
    if (!this.boardEmployees.includes(employee)) {
      this.boardEmployees = [...this.boardEmployees, employee];
      this.employeesTable.data = this.boardEmployees;
    }
  }

  onRemovingEmployee(employee: Employee) {
    this.employeeService.removeEmployeeFromTheBoard(this.boardId, employee.id)
        .subscribe(() => {
          if (this.boardEmployees.includes(employee)) {
            this.boardEmployees = this.boardEmployees.filter(e => e !== employee);
            this.employeesTable.data = this.boardEmployees;
            this.filter.nativeElement['value'] = '';
    }});
  }
}

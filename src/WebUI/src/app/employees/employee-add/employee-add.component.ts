import { Component, ElementRef, EventEmitter, Input, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { EmployeeService } from '../employee.service';
import { ActivatedRoute } from '@angular/router';
import { MatSort } from '@angular/material/sort';
import { Employee } from 'src/app/models/employee';
import { RolesService } from 'src/app/users/roles.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableHelper } from 'src/app/common/helpers/mat-table-helper';

@Component({
  selector: 'tt-employee-add',
  templateUrl: './employee-add.component.html',
  styleUrls: ['./employee-add.component.scss']
})
export class EmployeeAddComponent implements OnInit {
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild('filter') filter!: ElementRef;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @Input() boardEmployees: Employee[] = [];
  @Output() employeeAdded = new EventEmitter<Employee>();
  tableHelper = new MatTableHelper<Employee>();
  
  boardId!: number;
  employeesTable!: MatTableDataSource<Employee>;
  allEmployees!: Employee[];

  constructor(private employeeService: EmployeeService,
    public rolesService: RolesService,
    private activatedRoute: ActivatedRoute) { 

  }

  ngOnInit(): void {
    this.boardId = Number(this.activatedRoute.snapshot.paramMap.get("boardId"));
    this.loadEmployees();
  }

  ngAfterViewInit() {
    this.tableHelper.initiateTable(this.employeesTable, this.sort, this.paginator);
  }

  ngOnChanges(changes: SimpleChanges) {
    if(changes['boardEmployees'] && this.allEmployees)
      this.employeesTable.data = this.filterEmployees();
  }

  filterEmployees(): Employee[] {
    return this.allEmployees.filter((e => !this.boardEmployees
      .some(be => be.userName === e.userName)))
  }

  loadEmployees() {
    this.employeeService.getAllEmployees()
      .subscribe(result => {
        this.allEmployees = result;
        this.employeesTable = new MatTableDataSource(this.filterEmployees());
        this.tableHelper.initiateTable(this.employeesTable, this.sort, this.paginator);
      });
  }

  onFilterTextChanged(filterText: string) {
    this.employeesTable.data = this.filterEmployees()
      .filter(x => x.userName.toLowerCase()
        .includes(filterText.toLowerCase()) 
          || (x.lastName?.includes(filterText.toLowerCase()) ?? false)
          || (x.firstName?.includes(filterText.toLowerCase()) ?? false)
          || (x.roles.filter(r => r.toLowerCase().includes(filterText.toLowerCase())).length > 0));
  }

  clearFilter() {
    this.filter.nativeElement['value'] = '';
    this.onFilterTextChanged('');
  }

  onAddEmployeeToBoard(employee: Employee) {
    this.employeeService.addEmployeeToTheBoard(this.boardId.toString(), employee.userName)
      .subscribe(() => {
        this.employeeAdded.emit(employee);
        this.filter.nativeElement['value'] = '';
      });
  }
}

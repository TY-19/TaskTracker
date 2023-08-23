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
  
  boardId!: number;
  allEmployees!: Employee[];
  employeesTable!: MatTableDataSource<Employee>;
  tableHelper = new MatTableHelper<Employee>();

  constructor(private employeeService: EmployeeService,
    private rolesService: RolesService,
    private activatedRoute: ActivatedRoute) { 

  }

  ngOnInit(): void {
    this.boardId = Number(this.activatedRoute.snapshot.paramMap.get("boardId"));
    this.loadEmployees();
  }

  ngAfterViewInit(): void {
    this.tableHelper.initiateTable(this.employeesTable, this.sort, this.paginator);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(changes['boardEmployees'] && this.allEmployees)
      this.employeesTable.data = this.filterTheBoardEmployees();
  }

  private filterTheBoardEmployees(): Employee[] {
    return this.allEmployees.filter((e => !this.boardEmployees
      .some(be => be.userName === e.userName)))
  }

  private loadEmployees(): void {
    this.employeeService.getAllEmployees()
      .subscribe(result => {
        this.allEmployees = result;
        this.employeesTable = new MatTableDataSource(this.filterTheBoardEmployees());
        this.tableHelper.initiateTable(this.employeesTable, this.sort, this.paginator);
      });
  }

  onFilterTextChanged(filterText: string): void {
    this.employeesTable.data = this.employeeService
      .filterEmployees(filterText, this.filterTheBoardEmployees());
  }

  clearFilter(): void {
    this.filter.nativeElement['value'] = '';
    this.onFilterTextChanged('');
  }

  onAddEmployeeToBoard(employee: Employee): void {
    this.employeeService.addEmployeeToTheBoard(this.boardId, employee.userName)
      .subscribe(() => {
        this.employeeAdded.emit(employee);
        this.filter.nativeElement['value'] = '';
      });
  }

  isAdmin(roles: string[]): boolean {
    return this.rolesService.isAdmin(roles);
  }
  get adminRoleName(): string {
    return this.rolesService.adminRole;
  }

  isManager(roles: string[]): boolean {
    return this.rolesService.isManager(roles);
  }
  get managerRoleName(): string {
    return this.rolesService.managerRole;
  }
}

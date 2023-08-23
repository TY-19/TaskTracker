import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { EmployeeService } from './employee.service';
import { ActivatedRoute } from '@angular/router';
import { Employee } from '../models/employee';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { RolesService } from '../users/roles.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableHelper } from '../common/helpers/mat-table-helper';
import { DisplayModes } from '../common/enums/display-modes';

@Component({
  selector: 'tt-employees',
  templateUrl: './employees.component.html',
  styleUrls: ['./employees.component.scss']
})
export class EmployeesComponent implements OnInit {
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild('filter') filter!: ElementRef;
  
  boardId!: string;
  boardEmployees: Employee[] = [];

  employeesTable!: MatTableDataSource<Employee>;
  private tableHelper = new MatTableHelper<Employee>();

  showPanel: boolean = false;
  mode: DisplayModes = DisplayModes.View;

  constructor(private employeeService: EmployeeService,
    private rolesService: RolesService,
    private activatedRoute: ActivatedRoute) { 

  }

  ngOnInit(): void {
    this.boardId = this.activatedRoute.snapshot.paramMap.get('boardId')!;
    this.loadEmployees();
  }

  ngAfterViewInit(): void {
    this.tableHelper.initiateTable(this.employeesTable, this.sort, this.paginator);
  }

  private loadEmployees(): void {
    this.employeeService.getEmployees(this.boardId)
      .subscribe(result => {
        this.boardEmployees = result;
        this.employeesTable = new MatTableDataSource<Employee>(result);
        this.tableHelper.initiateTable(this.employeesTable, this.sort, this.paginator);
      });
  }
  
  get isInEditMode(): boolean {
    return this.mode === DisplayModes.Edit;
  }

  setModeToView(): void {
    this.changeMode(DisplayModes.View);
  }

  setModeToEdit(): void {
    this.changeMode(DisplayModes.Edit);
  }

  private changeMode(mode: DisplayModes): void {
    this.mode = mode;
    this.showPanel = this.isInEditMode;
  }

  onFilterTextChanged(filterText: string): void {
    this.employeesTable.data = this.employeeService
      .filterEmployees(filterText, this.boardEmployees);
  }

  clearFilter(): void {
    this.filter.nativeElement['value'] = '';
    this.onFilterTextChanged('');
  }

  onAddingEmployee(employee: Employee): void {
    if (!this.boardEmployees.includes(employee)) {
      this.boardEmployees = [...this.boardEmployees, employee];
      this.employeesTable.data = this.boardEmployees;
    }
  }

  onRemovingEmployee(employee: Employee): void {
    this.employeeService.removeEmployeeFromTheBoard(this.boardId, employee.id)
        .subscribe(() => this.refreshEmployees(employee));
  }

  private refreshEmployees(employee: Employee): void {
    if (this.boardEmployees.includes(employee)) {
      this.boardEmployees = this.boardEmployees.filter(e => e !== employee);
      this.employeesTable.data = this.boardEmployees;
      this.filter.nativeElement['value'] = '';
    }
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

import { ChangeDetectorRef, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { EmployeeService } from './employee.service';
import { ActivatedRoute } from '@angular/router';
import { Employee } from '../models/employee';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { DefaultRolesNames } from '../config/default-roles-names';

@Component({
  selector: 'tt-employees',
  templateUrl: './employees.component.html',
  styleUrls: ['./employees.component.scss']
})
export class EmployeesComponent implements OnInit {
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild('filter') filter!: ElementRef;
  sortInitiated: boolean = false;
  
  boardId: string = "";
  boardEmployees: Employee[] = [];
  employees!: MatTableDataSource<Employee>;

  showPanel: boolean = false;
  mode: string = "view";

  constructor(private employeeService: EmployeeService,
    private activatedRoute: ActivatedRoute,
    private changeDet: ChangeDetectorRef) { 

  }

  ngOnInit(): void {
    this.boardId = this.activatedRoute.snapshot.paramMap.get('boardId')!;
    this.getEmployees();
  }

  ngAfterViewInit() {
    this.initiateSort();
  }

  getEmployees() {
    this.employeeService.getEmployees(this.boardId)
      .subscribe(result => {
        this.boardEmployees = result;
        this.employees = new MatTableDataSource<Employee>(result);
        this.initiateSort();
      });
  }

  private initiateSort() {
    if (!this.sortInitiated && this.employees && this.sort) {
        this.sort.disableClear = true;
        this.employees.sort = this.sort;
        this.employees.sortingDataAccessor = 
          (data, sortHeaderId) => data[sortHeaderId].toLowerCase();
        this.sortInitiated = true;
    }
  }
  
  changeMode(mode: string) {
    this.mode = mode;
    if (mode === 'create' || mode === 'edit')    
        this.showPanel = true;
    else
        this.showPanel = false;
  }

  onFilterTextChanged(filterText: string) {
    this.employees.data = this.boardEmployees
      .filter(x => x.userName.toLowerCase()
        .includes(filterText.toLowerCase()) 
          || (x.lastName?.includes(filterText.toLowerCase()) ?? false)
          || (x.firstName?.includes(filterText.toLowerCase()) ?? false)
          || (x.roles.filter(r => r.toLowerCase().includes(filterText.toLowerCase())).length > 0));
  }

  onAddingEmployee(employee: Employee) {
    if (!this.boardEmployees.includes(employee)) {
      this.boardEmployees = [...this.boardEmployees, employee];
      this.employees.data = this.boardEmployees;
    }
  }

  onRemovingEmployee(employee: Employee) {
    this.employeeService.removeEmployeeFromTheBoard(this.boardId, employee.id)
        .subscribe(() => {
          if (this.boardEmployees.includes(employee)) {
            this.boardEmployees = this.boardEmployees.filter(e => e !== employee);
            this.employees.data = this.boardEmployees;
            this.filter.nativeElement['value'] = '';
    }});
  }
  
  isAdmin(roles: string[]) {
    return roles.includes(DefaultRolesNames.DEFAULT_ADMIN_ROLE);
  }

  isManager(roles: string[]) {
    return roles.includes(DefaultRolesNames.DEFAULT_MANAGER_ROLE);
  }
}

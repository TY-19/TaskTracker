import { Component, ElementRef, EventEmitter, Input, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { EmployeeService } from '../employee.service';
import { ActivatedRoute } from '@angular/router';
import { MatSort } from '@angular/material/sort';
import { DefaultRolesNames } from 'src/app/config/default-roles-names';
import { Employee } from 'src/app/models/employee';

@Component({
  selector: 'tt-employee-add',
  templateUrl: './employee-add.component.html',
  styleUrls: ['./employee-add.component.scss']
})
export class EmployeeAddComponent implements OnInit {
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild('filter') filter!: ElementRef;
  @Input() boardEmployees: Employee[] = [];
  @Output() employeeAdded = new EventEmitter<Employee>()
  
  boardId!: number;
  employeesList!: MatTableDataSource<Employee>;
  employees!: Employee[];

  constructor(private employeeService: EmployeeService,
    private activatedRoute: ActivatedRoute) { 

  }

  ngOnInit(): void {
    this.boardId = Number(this.activatedRoute.snapshot.paramMap.get("boardId"));
    this.loadEmployees();
  }

  ngAfterViewInit() {
    this.initiateSort();
  }

  ngOnChanges(changes: SimpleChanges) {
    if(changes['boardEmployees'] && this.employees)
      this.employeesList.data = this.filterEmployees();
  }

  filterEmployees(): Employee[] {
    return this.employees.filter((e => !this.boardEmployees
      .some(be => be.userName === e.userName)))
  }

  loadEmployees() {
    this.employeeService.getAllEmployees()
      .subscribe(result => {
        this.employees = result;
        this.employeesList = new MatTableDataSource(this.filterEmployees());
        this.initiateSort();
      });
  }

  private initiateSort() {
    if(this.employeesList && this.sort && this.employeesList.sort !== this.sort) {
      this.sort.disableClear = true;
      this.employeesList.sort = this.sort;
      this.employeesList.sortingDataAccessor = (data, sortHeaderId) => data[sortHeaderId].toLowerCase();
    }
  }

  onFilterTextChanged(filterText: string) {
    this.employeesList.data = this.filterEmployees()
      .filter(x => x.userName.toLowerCase()
        .includes(filterText.toLowerCase()) 
          || (x.lastName?.includes(filterText.toLowerCase()) ?? false)
          || (x.firstName?.includes(filterText.toLowerCase()) ?? false)
          || (x.roles.filter(r => r.toLowerCase().includes(filterText.toLowerCase())).length > 0));
  }

  onAddEmployeeToBoard(employee: Employee) {
    this.employeeService.addEmployeeToTheBoard(this.boardId.toString(), employee.userName)
      .subscribe(() => {
        this.employeeAdded.emit(employee);
        console.log(this.filter);
        this.filter.nativeElement['value'] = '';
      });
  }

  isAdmin(roles: string[]) {
    return roles.includes(DefaultRolesNames.DEFAULT_ADMIN_ROLE);
  }

  isManager(roles: string[]) {
    return roles.includes(DefaultRolesNames.DEFAULT_MANAGER_ROLE);
  }
}

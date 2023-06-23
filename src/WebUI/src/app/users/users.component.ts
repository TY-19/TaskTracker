import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { UserService } from './user.service';
import { MatTableDataSource } from '@angular/material/table';
import { UserProfile } from '../models/user-profile';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { RolesService } from './roles.service';
import { MatTableHelper } from '../common/helpers/mat-table-helper';

@Component({
  selector: 'tt-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit {
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild('filter') filter!: ElementRef;
  tableHelper = new MatTableHelper<UserProfile>();
  users!: UserProfile[];
  usersTable! : MatTableDataSource<UserProfile>;
  

  constructor(private userSevice: UserService,
    public rolesService: RolesService) { 

  }

  ngOnInit(): void {
    this.loadUsers();
  }

  ngAfterViewInit() {
    this.tableHelper.initiateTable(this.usersTable, this.sort, this.paginator);
  }

  loadUsers() {
    this.userSevice.getUsers()
      .subscribe(result => {
        this.users = result
        this.usersTable = new MatTableDataSource(this.users);
        this.tableHelper.initiateTable(this.usersTable, this.sort, this.paginator);
      });
  }

  onFilterTextChanged(filterText: string) {
    this.usersTable.data = this.users
      .filter(x => x.userName.toLowerCase().includes(filterText.toLowerCase()) 
          || (x.lastName?.includes(filterText.toLowerCase()) ?? false)
          || (x.firstName?.includes(filterText.toLowerCase()) ?? false)
          || (x.email?.includes(filterText.toLowerCase()))
          || (x.roles.filter(r => r.toLowerCase().includes(filterText.toLowerCase())).length > 0));
  }

  filterByRole(role: string) {
    this.filter.nativeElement['value'] = role;
    this.onFilterTextChanged(role);
  }

  clearFilter() {
    this.filter.nativeElement['value'] = '';
    this.onFilterTextChanged('');
  }

}

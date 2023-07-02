import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { AssignmentService } from './assignment.service';
import { ActivatedRoute } from '@angular/router';
import { Assignment } from '../models/assignment';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableHelper } from '../common/helpers/mat-table-helper';

@Component({
  selector: 'tt-assignments',
  templateUrl: './assignments.component.html',
  styleUrls: ['./assignments.component.scss']
})
export class AssignmentsComponent implements OnInit {
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild('filter') filter!: ElementRef;

  tableHelper = new MatTableHelper<Assignment>();

  boardId: string = "";
  assignments: Assignment[] = [];
  assignmentsTable!: MatTableDataSource<Assignment>;

  constructor(private assignmetnService: AssignmentService,
    private activatedRoute: ActivatedRoute) { 

  }

  ngOnInit(): void {
    this.boardId = this.activatedRoute.snapshot.paramMap.get('boardId')!;
    this.getAssignments();
  }

  ngAfterViewInit() {
    this.tableHelper.initiateTable(this.assignmentsTable, this.sort, this.paginator);
  }

  getAssignments() {
    this.assignmetnService.getAssignments(this.boardId)
      .subscribe(result => {
        this.assignments = result;
        this.assignmentsTable = new MatTableDataSource<Assignment>(result);
        this.tableHelper.initiateTable(this.assignmentsTable, this.sort, this.paginator);
      });
  }

  onFilterTextChanged(filterText: string) {
    this.assignmentsTable.data = this.assignments
      .filter(x => x.topic.toLowerCase().includes(filterText.toLowerCase()));
  }

  clearFilter() {
    this.filter.nativeElement['value'] = '';
    this.onFilterTextChanged('');
  }

}

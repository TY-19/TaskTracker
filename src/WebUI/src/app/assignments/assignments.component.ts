import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { AssignmentService } from './assignment.service';
import { ActivatedRoute } from '@angular/router';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableHelper } from '../common/helpers/mat-table-helper';
import { Stage } from '../models/stage';
import { Employee } from '../models/employee';
import { AssignmentDisplayModel } from '../models/display-models/assignment-display-model';
import { BoardService } from '../boards/board.service';
import { Board } from '../models/board';
import { AssignmentDisplayModelSortingDataAccessor } from '../common/helpers/sorting-helpers';

@Component({
  selector: 'tt-assignments',
  templateUrl: './assignments.component.html',
  styleUrls: ['./assignments.component.scss']
})
export class AssignmentsComponent implements OnInit {
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild('filter') filter!: ElementRef;

  tableHelper = new MatTableHelper<AssignmentDisplayModel>(
    AssignmentDisplayModelSortingDataAccessor
  );

  boardId: string = "";
  board!: Board;

  assignmentsModels: AssignmentDisplayModel[] = [];
  assignmentsTable!: MatTableDataSource<AssignmentDisplayModel>;

  stages: Stage[] = [];
  employees: Employee[] = []

  constructor(private assignmetnService: AssignmentService,
    private boardService: BoardService,
    private activatedRoute: ActivatedRoute) { 

  }

  ngOnInit(): void {
    this.boardId = this.activatedRoute.snapshot.paramMap.get('boardId')!;
    this.loadBoard();
  }

  ngAfterViewInit() {
    this.tableHelper.initiateTable(this.assignmentsTable, this.sort, this.paginator);
  }

  loadBoard() {
    this.boardService.getBoard(this.boardId)
      .subscribe(result => {
        this.board = result;
        this.assignmentsModels = this.getDataSource();
        this.assignmentsTable = new MatTableDataSource<AssignmentDisplayModel>(
          this.assignmentsModels);
        this.tableHelper.initiateTable(this.assignmentsTable, this.sort, this.paginator);
      });
  }

  reloadBoard() {
    this.boardService.getBoard(this.boardId)
      .subscribe(result => {
        this.board = result;
        this.assignmentsModels = this.getDataSource();
        this.assignmentsTable.data = this.assignmentsModels;
      });
  }

  getDataSource(): AssignmentDisplayModel[] {
    if (!this.board.assignments)
      return [];
    let assignmentDetails: AssignmentDisplayModel[] = [];
    for (let assignment of this.board.assignments) {
      let detail: AssignmentDisplayModel = {
        id: assignment.id,
        boardId: assignment.boardId,
        topic: assignment.topic,
        description: assignment.description,
        deadline: assignment.deadline,
        isCompleted: assignment.isCompleted,
        stage: this.getStage(assignment.stageId),
        responsibleEmployee: this.getEmployee(assignment.responsibleEmployeeId),
      }
      assignmentDetails.push(detail);
    }
    return assignmentDetails;
  }

  private getStage(stageId: number): Stage | undefined {
    return this.board.stages?.find(s => s.id == stageId);
  }

  private getEmployee(employeeId: number): Employee | undefined {
    return this.board.employees?.find(e => e.id == employeeId);
  }

  onFilterTextChanged(filterText: string) {
    this.assignmentsTable.data = this.assignmentsModels
      .filter(a => a.topic.toLowerCase().includes(filterText.toLowerCase())
        || this.filterByEmployeeName(a.responsibleEmployee, filterText));
  }

  selectEmployeesTasks(employeeName: string) {
    this.filter.nativeElement['value'] = employeeName;
    this.onFilterTextChanged(employeeName);
  }

  filterByEmployeeName(employee: Employee | undefined, filterText: string): boolean {
    if (employee === undefined) return false;
    let firstName = employee.firstName?.toLowerCase() ?? '';
    let lastName = employee.lastName?.toLowerCase() ?? '';
    let fullName = firstName + ' ' + lastName;
    return fullName.includes(filterText.toLowerCase());
  }

  clearFilter() {
    this.filter.nativeElement['value'] = '';
    this.onFilterTextChanged('');
  }

  deleteAssignment(assignmentId: number) {
    this.assignmetnService.deleteAssignment(this.boardId, assignmentId.toString())
      .subscribe(() => {
        this.reloadBoard();
      })
  }

}

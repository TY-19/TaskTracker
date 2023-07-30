import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { AssignmentService } from '../assignment.service';
import { BoardService } from 'src/app/boards/board.service';
import { AssignmentDisplayModel } from 'src/app/models/display-models/assignment-display-model';
import { MatTableDataSource } from '@angular/material/table';
import { Stage } from 'src/app/models/stage';
import { Employee } from 'src/app/models/employee';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableHelper } from 'src/app/common/helpers/mat-table-helper';
import { AssignmentDisplayModelSortingDataAccessor } from 'src/app/common/helpers/sorting-helpers';
import { Board } from 'src/app/models/board';
import { AuthService } from 'src/app/auth/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'tt-assignments-all-boards',
  templateUrl: './assignments-all-boards.component.html',
  styleUrls: ['./assignments-all-boards.component.scss']
})
export class AssignmentsAllBoardsComponent implements OnInit {

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild('filter') filter!: ElementRef;

  tableHelper = new MatTableHelper<AssignmentDisplayModel>(
    AssignmentDisplayModelSortingDataAccessor
  );

  boards!: Board[];

  assignmentsModels: AssignmentDisplayModel[] = [];
  assignmentsTable!: MatTableDataSource<AssignmentDisplayModel>;

  stages: Stage[] = [];
  employees: Employee[] = [];
  displayedColumns = ['topic', 'description', 'deadline', 'board', 'stage', 'buttons'];
  showCreateMenu: boolean = false;

  constructor(private assignmetnService: AssignmentService,
    private boardService: BoardService,
    public authService: AuthService,
    private router: Router) { 

  }

  ngOnInit(): void {
    this.loadBoards();
    if(this.authService.isAdmin() || this.authService.isManager()) {
      this.displayedColumns = ['topic', 'description', 'deadline', 
        'board', 'stage', 'responsibleEmployee', 'buttons'];
    }
  }

  ngAfterViewInit() {
    this.tableHelper.initiateTable(this.assignmentsTable, this.sort, this.paginator);
  }

  loadBoards() {
    this.boardService.getBoardsOfTheEmployee()
      .subscribe(result => {
        this.boards = result;
        this.assignmentsModels = this.getDataSource();
        this.assignmentsTable = new MatTableDataSource<AssignmentDisplayModel>(
          this.assignmentsModels);
        this.tableHelper.initiateTable(this.assignmentsTable, this.sort, this.paginator);
      });
  }

  reloadBoards() {
    this.boardService.getBoardsOfTheEmployee()
      .subscribe(result => {
        this.boards = result;
        this.assignmentsModels = this.getDataSource();
        this.assignmentsTable.data = this.assignmentsModels;
      });
  }

  getDataSource(): AssignmentDisplayModel[] {
    let assignmentDetails: AssignmentDisplayModel[] = [];
    for (let board of this.boards)
    {
      if (!board.assignments)
        continue;
      
      for (let assignment of board.assignments) {
        let detail: AssignmentDisplayModel = {
          id: assignment.id,
          boardId: assignment.boardId,
          boardName: board.name,
          topic: assignment.topic,
          description: assignment.description,
          deadline: assignment.deadline,
          isCompleted: assignment.isCompleted,
          stage: this.getStage(board, assignment.stageId),
          responsibleEmployee: this.getEmployee(board, assignment.responsibleEmployeeId)
        }
        assignmentDetails.push(detail);
      }
    }
    if (this.authService.isAdmin() || this.authService.isManager()) {
      return assignmentDetails;
    }
    let employeeId = this.authService.getEmployeeId()
    if (!employeeId) {
      return [];
    }
    return assignmentDetails.filter(ad => ad.responsibleEmployee 
      && ad.responsibleEmployee.id.toString() == employeeId)
  }

  private getStage(board: Board, stageId: number): Stage | undefined {
    return board.stages?.find(s => s.id == stageId);
  }

  private getEmployee(board: Board, employeeId: number): Employee | undefined {
    return board.employees?.find(e => e.id == employeeId);
  }

  onFilterTextChanged(filterText: string) {
    this.assignmentsTable.data = this.assignmentsModels
      .filter(a => a.topic.toLowerCase().includes(filterText.toLowerCase()));
  }

  clearFilter() {
    this.filter.nativeElement['value'] = '';
    this.onFilterTextChanged('');
  }

  createAssignment(boardId: number) {
    this.router.navigate(['/boards', boardId, 'tasks', 'create']);
  }

  deleteAssignment(boardId: number, assignmentId: number) {
    this.assignmetnService
      .deleteAssignment(boardId.toString(), assignmentId.toString())
      .subscribe(() => this.reloadBoards());
  }
}

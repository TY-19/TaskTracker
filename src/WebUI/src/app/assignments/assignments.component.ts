import { AfterViewInit, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { AuthService } from '../auth/auth.service';
import { AssignmentsModes } from '../common/enums/assignments-modes';
import { AssignmentDisplayModelSortingDataAccessor } from '../common/helpers/sorting-helpers';
import { MatTableHelper } from '../common/helpers/mat-table-helper';
import { BoardService } from '../boards/board.service';
import { AssignmentService } from './assignment.service';
import { Board } from '../models/board';
import { AssignmentDisplayModel } from '../models/display-models/assignment-display-model';
import { Employee } from '../models/employee';
import { Stage } from '../models/stage';
import { AssignmentDisplayService } from './assignment-display.service';

@Component({
  selector: 'tt-assignments',
  templateUrl: './assignments.component.html',
  styleUrls: ['./assignments.component.scss']
})
export class AssignmentsComponent implements OnInit, AfterViewInit {
  @Input() mode: AssignmentsModes = AssignmentsModes.SingleBoard;
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild('filter') filter!: ElementRef;

  boardId: string | null = null;
  boards: Board[] = [];
  stages: Stage[] = [];
  employees: Employee[] = [];

  assignmentsModels: AssignmentDisplayModel[] = [];
  assignmentsTable?: MatTableDataSource<AssignmentDisplayModel>;
  private tableHelper = new MatTableHelper<AssignmentDisplayModel>(
    AssignmentDisplayModelSortingDataAccessor
  );
  displayedColumns = ['topic', 'description', 'deadline', 'board', 'stage', 'buttons'];

  constructor(private authService: AuthService,
    private assignmentService: AssignmentService,
    private assignmentDisplayService: AssignmentDisplayService,
    private boardService: BoardService,
    private activatedRoute: ActivatedRoute) { 

  }

  ngOnInit(): void {
    this.boardId = this.activatedRoute.snapshot.paramMap.get('boardId');
    this.setDisplayedColumns();
    this.setMode();
    this.loadBoards();
  }

  ngAfterViewInit() {
    this.tableHelper.initiateTable(this.assignmentsTable, this.sort, this.paginator);
  }

  get isInSingleBoardMode(): boolean {
    return this.mode === AssignmentsModes.SingleBoard;
  }
  get isInMultyBoardsMode(): boolean {
    return this.mode === AssignmentsModes.MultyBoards;
  }

  get isAdminOrManager(): boolean {
    return this.authService.isAdmin() || this.authService.isManager();
  }

  private setMode(): void {
    this.mode = this.boardId ? AssignmentsModes.SingleBoard : AssignmentsModes.MultyBoards;
  }

  private setDisplayedColumns(): void {
    if(this.mode === AssignmentsModes.SingleBoard) {
      this.displayedColumns = ['topic', 'description', 'deadline', 'stage',
        'responsibleEmployee', 'buttons']
    } else if(this.authService.isAdmin() || this.authService.isManager()) {
      this.displayedColumns = ['topic', 'description', 'deadline',
        'board', 'stage', 'responsibleEmployee', 'buttons'];
    } else {
      this.displayedColumns = ['topic', 'description', 'deadline',
        'board', 'stage', 'buttons'];
    }
  }

  loadBoards(): void {
    if(this.boardId) {
      this.loadSingleBoard();
    } else {
      this.loadMultipleBoards();
    }
  }

  private loadSingleBoard(): void {
    this.boardService.getBoard(this.boardId!)
      .subscribe(result => {
        this.boards.push(result);
        this.buildTable();
      });
  }

  private loadMultipleBoards(): void {
    this.boardService.getBoardsOfTheEmployee()
      .subscribe(result => {
        this.boards = result;
        this.buildTable();
      });
  }

  private buildTable(): void {
    this.assignmentsModels = this.assignmentDisplayService.getAssignmentDisplayModels(this.boards);
    if(this.assignmentsTable) {
      this.assignmentsTable.data = this.assignmentsModels;
    } else {
      this.assignmentsTable = new MatTableDataSource<AssignmentDisplayModel>(this.assignmentsModels);
      this.tableHelper.initiateTable(this.assignmentsTable, this.sort, this.paginator);
    }
  }

  onFilterTextChanged(filterText: string): void {
    this.assignmentsTable!.data = this.assignmentsModels
      .filter(a => a.topic.toLowerCase().includes(filterText.toLowerCase())
        || this.filterByEmployeeName(a.responsibleEmployee, filterText));
  }

  private filterByEmployeeName(employee: Employee | undefined, filterText: string): boolean {
    if (employee === undefined) {
      return false;
    }
    const firstName = employee.firstName?.toLowerCase() ?? '';
    const lastName = employee.lastName?.toLowerCase() ?? '';
    const fullName = firstName + ' ' + lastName;
    return fullName.includes(filterText.toLowerCase());
  }

  clearFilter(): void {
    this.filter.nativeElement['value'] = '';
    this.onFilterTextChanged('');
  }

  selectEmployeesTasks(employeeName: string): void {
    this.filter.nativeElement['value'] = employeeName;
    this.onFilterTextChanged(employeeName);
  }

  deleteAssignment(boardId: number, assignmentId: number): void {
    this.assignmentService
      .deleteAssignment(boardId, assignmentId.toString())
      .subscribe(() => this.loadBoards());
  }
}

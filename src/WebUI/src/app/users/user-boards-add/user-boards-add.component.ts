import { AfterViewInit, Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';
import { BoardService } from 'src/app/boards/board.service';
import { MatTableHelper } from 'src/app/common/helpers/mat-table-helper';
import { EmployeeService } from 'src/app/employees/employee.service';
import { Board } from 'src/app/models/board';

@Component({
  selector: 'tt-user-boards-add',
  templateUrl: './user-boards-add.component.html',
  styleUrls: ['./user-boards-add.component.scss']
})
export class UserBoardsAddComponent implements OnInit, AfterViewInit {
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild('filter') filter!: ElementRef;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  @Input() employeeId?: number;
  @Output() boardAdded = new EventEmitter<number>();

  userName!: string;
  boards: Board[] = [];
  boardsTable!: MatTableDataSource<Board>;
  tableHelper = new MatTableHelper<Board>();

  constructor(private boardService: BoardService,
    private employeeService: EmployeeService,
    private activatedRoute: ActivatedRoute) { 

  }

  ngOnInit(): void {
    this.userName = this.activatedRoute.snapshot.paramMap.get("userName")!;
    this.loadBoards();
  }

  ngAfterViewInit(): void {
    this.tableHelper.initiateTable(this.boardsTable, this.sort, this.paginator);
  }

  private loadBoards(): void {
    this.boardService.getBoards()
      .subscribe(result => {
        this.boards = result;
        this.boardsTable = new MatTableDataSource(this.filterBoards());
        this.tableHelper.initiateTable(this.boardsTable, this.sort, this.paginator);
      })
  }

  reloadBoards(): void {
    this.boardService.getBoards()
      .subscribe(result => {
        this.boards = result;
        this.boardsTable.data = this.filterBoards();
      })
  }

  filterBoards(): Board[] {
    return this.boards.filter(b => !b.employees || b.employees.length === 0 
      || !b.employees?.some(e => e.id == this.employeeId));
  }

  onAddUserToBoard(boardId: number): void {
    this.employeeService.addEmployeeToTheBoard(boardId.toString(), this.userName)
      .subscribe(() => {
        this.reloadBoards();
        this.boardAdded.emit(boardId);
      });
  }

  onFilterTextChanged(filterText: string): void {
    this.boardsTable.data = this.filterBoards()
      .filter(b => b.name.toLowerCase().includes(filterText.toLowerCase()) 
        || b.id.toString().toLowerCase().includes(filterText.toLowerCase()));
  }

  clearFilter(): void {
    this.filter.nativeElement['value'] = '';
    this.onFilterTextChanged('');
  }
}

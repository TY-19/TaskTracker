import { Component, OnInit, ViewChild } from '@angular/core';
import { Board } from 'src/app/models/board';
import { UserProfile } from 'src/app/models/user-profile';
import { UserService } from '../user.service';
import { BoardService } from 'src/app/boards/board.service';
import { ActivatedRoute } from '@angular/router';
import { EmployeeService } from 'src/app/employees/employee.service';
import { UserBoardsAddComponent } from '../user-boards-add/user-boards-add.component';
import { Assignment } from 'src/app/models/assignment';
import { DisplayModes } from 'src/app/common/enums/display-modes';

@Component({
  selector: 'tt-user-boards',
  templateUrl: './user-boards.component.html',
  styleUrls: ['./user-boards.component.scss']
})
export class UserBoardsComponent implements OnInit {
  @ViewChild(UserBoardsAddComponent) addBoardComponent!: UserBoardsAddComponent;
  userName!: string;
  user?: UserProfile;
  userBoards?: Board[];
  mode: DisplayModes = DisplayModes.View;
  
  constructor(private userService: UserService,
    private employeeService: EmployeeService,
    private boardService: BoardService,
    private activatedRoute: ActivatedRoute) { 

  }

  ngOnInit(): void {
    this.userName = this.activatedRoute.snapshot.paramMap.get("userName")!;
    this.userService.getUser(this.userName)
      .subscribe(result => { 
        this.user = result;
        this.loadBoards();
      });
  }

  private loadBoards(): void {
    this.boardService.getBoards()
      .subscribe(result => {
        this.userBoards = result.filter(b => b.employees?.some(e => e.id == this.user?.employeeId));
      })
  }

  getUserAssignments(board: Board): Assignment[] {
    return board.assignments?.filter(a => a.responsibleEmployeeId === this.user?.employeeId) ?? [];
  }

  get isInViewMode(): boolean {
    return this.mode === DisplayModes.View;
  }
  get isInCreateMode(): boolean {
    return this.mode === DisplayModes.Create;
  }

  changeModeToView(): void {
    this.changeMode(DisplayModes.View);
  }
  changeModeToCreate(): void {
    this.changeMode(DisplayModes.Create);
  }
  private changeMode(mode: DisplayModes): void {
    this.mode = mode;
  }

  onBoardAdded(): void {
    this.loadBoards();
  }

  onBoardRemoved(boardId: number): void {
    if (this.user?.employeeId) {
      this.employeeService.removeEmployeeFromTheBoard(boardId.toString(), this.user?.employeeId)
        .subscribe(() => {
          this.loadBoards();
          this.addBoardComponent.reloadBoards();
        });
    }
  }
}

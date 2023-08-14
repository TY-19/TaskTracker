import { Component, OnInit, ViewChild } from '@angular/core';
import { BoardService } from '../board.service';
import { Board } from 'src/app/models/board';
import { ActivatedRoute } from '@angular/router';
import { CdkDragDrop } from '@angular/cdk/drag-drop';
import { Assignment } from 'src/app/models/assignment';
import { AssignmentService } from 'src/app/assignments/assignment.service';
import { Stage } from 'src/app/models/stage';
import { BoardAnimations, SidebarAnimations } from 'src/app/common/animations/sidebar-animation';
import { AuthService } from 'src/app/auth/auth.service';
import { BoardDisplayService } from './board-display-options/board-display.service';
import { DisplayModes } from 'src/app/common/enums/display-modes';
import { SidebarComponent } from './sidebar/sidebar.component';
import { BoardDisplayOptions } from './board-display-options/board-display-options';
import { AssignmentDisplayService } from 'src/app/assignments/assignment-display.service';

@Component({
  selector: 'tt-board-details',
  templateUrl: './board-details.component.html',
  styleUrls: ['./board-details.component.scss'],
  animations: [ 
    SidebarAnimations.showHideSidebar,
    BoardAnimations.moveBoard
  ]
})
export class BoardDetailsComponent implements OnInit {
  @ViewChild(SidebarComponent) sidebar?: SidebarComponent;

  board! : Board;
  currentTaskId?: number;

  showSidebar: boolean = false;
  showDisplayOption: boolean = false;
  showBoard: boolean = true;
  sidebarContent: DisplayModes = DisplayModes.View;
  boardDisplayOptions!: BoardDisplayOptions;

  get isAdminOrManager(): boolean {
    return this.authService.isAdmin() || this.authService.isManager();
  }

  constructor(private activatedRoute: ActivatedRoute,
    private boardService: BoardService,
    private assignmentService: AssignmentService,
    private assignmentDisplayService: AssignmentDisplayService,
    private authService: AuthService,
    boardDisplayService: BoardDisplayService) {
      this.boardDisplayOptions = new BoardDisplayOptions(boardDisplayService);
  }

  ngOnInit(): void {
    this.configureDisplayOptions();
    this.loadBoard();
  }

  private configureDisplayOptions(): void {
    this.boardDisplayOptions.applyDisplayOptions();
    this.refreshBoard();
  }

  private refreshBoard(): void {
    this.showBoard = false;
    this.showBoard = true;
  }

  loadBoard(): void {
    const idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.boardService.getBoard(idParam!)
      .subscribe(result => this.board = result);
  }

  getSortedStages(): Stage[] {
    return this.board.stages?.sort((a, b) => a.position - b.position) ?? [];
  }
  
  filterAssignmentOfTheStage(stageId: number): Assignment[] {
    const employeeId = Number(this.authService.getEmployeeId());
    return this.board.assignments?.filter(a => a.stageId == stageId
        && this.boardDisplayOptions.filterFunction(a, employeeId))
      .sort(this.boardDisplayOptions.sortingFunction) ?? [];
  }

  drop(event: CdkDragDrop<string[]>): void {
    if (!this.isAuthorizedToMoveTask(event.item.data as Assignment)) {
      return;
    }

    if (event.previousContainer !== event.container) {
      const stageId = event.container.element.nativeElement.getAttribute('stage-id');
      const assignment = event.item.data as Assignment;
      if (stageId && assignment)
      {
        assignment.stageId = Number(stageId);
        this.assignmentService
          .moveAssignmentToTheStage(this.board.id, assignment.id, assignment.stageId)
            .subscribe(() => {
              this.loadBoard();
              this.updateChildren(assignment);
            });
      }
    } 
  }

  private updateChildren(assignment: Assignment): void {
    if(this.sidebar) {
      this.sidebar.updateChildren(assignment);
    }
  }

  isAuthorizedToMoveTask(assignment: Assignment): boolean {
    return this.assignmentDisplayService.isUserAuthorizeToModifyTask(assignment);
  }

  viewTask(taskId: number): void {
    this.currentTaskId = taskId;
    this.showSidebar = true;
    this.sidebarContent = DisplayModes.View;
    if (this.sidebar) {
      this.sidebar.viewTask();
    }
  }

  createTask(): void {
    this.currentTaskId = undefined;
    this.showSidebar = true;
    this.sidebarContent = DisplayModes.Create;
    if (this.sidebar) {
      this.sidebar.createTask();
    }
  }

  getSidebarClass(): string {
    if (!this.showSidebar)
      return "full-page";

    switch(this.sidebarContent) {
      case DisplayModes.View: return "sidebar-view-margin";
      case DisplayModes.Create: return "sidebar-edit-margin";
      case DisplayModes.Edit: return "sidebar-edit-margin";
      default: return "full-page";
    }
  }

  getTaskClass(taskId: number): string {
    const responsibleEmployeeId = this.board.assignments
      ?.find(a => a.id == taskId)?.responsibleEmployeeId.toString();
    const currentUserEmployeeId = this.authService.getEmployeeId();
    const doesTaskBelongsToEmployee = currentUserEmployeeId && responsibleEmployeeId
      && currentUserEmployeeId === responsibleEmployeeId;
    return doesTaskBelongsToEmployee ? 'employees-task' : 'non-employees-task';
  }
}

import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { AssignmentDisplayService } from 'src/app/assignments/assignment-display.service';
import { AssignmentEditComponent } from 'src/app/assignments/assignment-edit/assignment-edit.component';
import { AssignmentViewComponent } from 'src/app/assignments/assignment-view/assignment-view.component';
import { AssignmentService } from 'src/app/assignments/assignment.service';
import { AuthService } from 'src/app/auth/auth.service';
import { DisplayModes } from 'src/app/common/enums/display-modes';
import { Assignment } from 'src/app/models/assignment';

@Component({
  selector: 'tt-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent implements OnInit {
  @ViewChild(AssignmentViewComponent) assignmentView!: AssignmentViewComponent;
  @ViewChild(AssignmentEditComponent) assignmentEdit!: AssignmentEditComponent;

  @Input() boardId!: number;
  @Input() currentTaskId?: number;
  
  @Input() showSidebar: boolean = false;
  @Output() showSidebarChange = new EventEmitter<boolean>();
  @Input() sidebarContent: DisplayModes = DisplayModes.View;
  @Output() sidebarContentChange = new EventEmitter<DisplayModes>();
  
  @Output() reloadBoard = new EventEmitter<void>();

  assignment?: Assignment;

  constructor(private authService: AuthService,
    private assignmentService: AssignmentService,
    private assignmentDisplayService: AssignmentDisplayService) {

  }

  ngOnInit(): void {
    if(this.currentTaskId && this.currentTaskId != 0) {
      this.assignmentService.getAssignment(this.boardId, this.currentTaskId)
        .subscribe(result => this.assignment = result);
    }
  }

  hideSidebar(): void {
    this.showSidebarChange.emit(false);
  }

  updateChildren(task: Assignment): void {
    if(this.currentTaskId == task.id) {
      if(this.sidebarContent == DisplayModes.View)
        this.assignmentView.loadStage(task.stageId);
      if(this.sidebarContent == DisplayModes.Edit)
        this.assignmentEdit.updateStage();
    }
  }

  viewTask(): void {
    if (this.assignmentView) {
      this.assignmentView?.ngOnInit();
    }
  }

  createTask(): void {
    if (this.assignmentEdit) {
      this.assignmentEdit.mode = DisplayModes.Create;
      this.assignmentEdit.assignmentId = "0";
      this.assignmentEdit?.ngOnInit();
    }
  }

  editTask(taskId: number): void {
    this.currentTaskId = taskId;
    this.showSidebar = true;
    this.sidebarContentChange.emit(DisplayModes.Edit);
  }

  async onSubmit(): Promise<void> {
    await new Promise<void>(resolve => {
      this.assignmentEdit.onSubmit();
      resolve();
    });
    if (this.assignmentEdit.isFormValid) {
      await new Promise(f => setTimeout(f, 300));
      this.reloadBoard.emit();
      this.showSidebarChange.emit(false);
    }
  }

  changeAssignmentStatus(assignment: Assignment): void {
    this.assignmentService
      .changeAssignmentStatus(assignment.boardId, assignment.id, !assignment.isCompleted)
        .subscribe(() => {
          this.assignmentView.loadAssignment(assignment.boardId, assignment.id);
        })
  }

  deleteAssignment(): void {
    this.assignmentService
      .deleteAssignment(this.boardId, this.currentTaskId!)
        .subscribe(() => {
          this.showSidebarChange.emit(false);
          this.reloadBoard.emit();
          this.currentTaskId = 0;
          this.sidebarContentChange.emit(DisplayModes.View);
        });
  }

  get isUserAuthorizeToChangeTaskStatus(): boolean {
    return this.assignment
      ? this.assignmentDisplayService.isUserAuthorizeToModifyTask(this.assignment)
      : false;
  }

  get isAdminOrManager(): boolean {
    return this.authService.isAdmin() || this.authService.isManager();
  }

  get isInViewMode(): boolean {
    return this.sidebarContent === DisplayModes.View;
  }

  get isInCreateMode(): boolean {
    return this.sidebarContent === DisplayModes.Create;
  }

  get isInEditMode(): boolean {
    return this.sidebarContent === DisplayModes.Edit;
  }
}

import { Component, OnInit, ViewChild } from '@angular/core';
import { BoardService } from '../board.service';
import { Board } from 'src/app/models/board';
import { ActivatedRoute } from '@angular/router';
import { CdkDragDrop } from '@angular/cdk/drag-drop';
import { Assignment } from 'src/app/models/assignment';
import { AssignmentService } from 'src/app/assignments/assignment.service';
import { Stage } from 'src/app/models/stage';
import { SidebarAnimations } from 'src/app/common/animations/sidebar-animation';
import { AssignmentEditComponent } from 'src/app/assignments/assignment-edit/assignment-edit.component';


@Component({
  selector: 'tt-board-details',
  templateUrl: './board-details.component.html',
  styleUrls: ['./board-details.component.scss'],
  animations: [ SidebarAnimations.animeTrigger ]
})
export class BoardDetailsComponent implements OnInit {
  @ViewChild(AssignmentEditComponent) assignmentEdit!: AssignmentEditComponent;
  board! : Board;
  showSidebar: boolean = false;
  sidebarContent: string = "details";

  currentTaskId?: number;
  
  constructor(private activatedRoute: ActivatedRoute,
    private boardService: BoardService,
    private assignmentService: AssignmentService) { 

  }

  ngOnInit(): void {
    this.getBoard();
  }

  getBoard() {
    let idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.boardService.getBoard(idParam!)
      .subscribe(result => this.board = result);
  }

  getSortedStages() : Stage[] {
    return this.board.stages?.sort((a, b) => a.position - b.position) ?? [];
  }

  filterAssignmentOfTheStage(stageId: number): Assignment[] {
    return this.board.assignments?.filter(a => a.stageId == stageId) ?? [];
  }

  drop(event: CdkDragDrop<string[]>) {
    if (event.previousContainer !== event.container) {
      let stageId = event.container.element.nativeElement.getAttribute('stage-id');
      let task = event.item.data;
      if (stageId && task)
      {
        (task as Assignment).stageId = Number(stageId);
        this.assignmentService.updateAssignment(this.board.id.toString(), task)
          .subscribe(() => this.getBoard());
      }
    } 
  }

  getSidebarClass(): string {
    if (!this.showSidebar)
      return "";
    switch(this.sidebarContent) {
      case "details": return "sidebar-view-margin";
      case "create": return "sidebar-edit-margin";
      case "edit": return "sidebar-edit-margin";
      default: return "";
    }
  }

  viewTask(taskId: number) {
    this.currentTaskId = taskId;
    this.showSidebar = true;
    this.sidebarContent = "details";
  }

  createTask() {
    this.currentTaskId = undefined;
    this.showSidebar = true;
    this.sidebarContent = "create";
  }

  editTask(taskId: number) {
    this.currentTaskId = taskId;
    this.showSidebar = true;
    this.sidebarContent = "edit";
  }

  async onSubmit() {
    this.showSidebar = false;
    await new Promise<void>(resolve => {
      this.assignmentEdit.onSubmit();
      resolve();
    });
    await new Promise(f => setTimeout(f, 300));
    this.getBoard();
  }

  deleteAssignment() {
    this.assignmentService.deleteAssignment(this.board.id.toString(), this.currentTaskId!.toString())
      .subscribe(() => {
        this.getBoard();
        this.currentTaskId = 0;
        this.showSidebar = false;
        this.sidebarContent = "details";
      });
  }
}

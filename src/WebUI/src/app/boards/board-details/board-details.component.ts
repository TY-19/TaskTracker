import { Component, OnInit } from '@angular/core';
import { BoardService } from '../board.service';
import { Board } from 'src/app/models/board';
import { ActivatedRoute } from '@angular/router';
import { CdkDragDrop, CdkDrag, CdkDropList, CdkDropListGroup,
  moveItemInArray,  transferArrayItem, } from '@angular/cdk/drag-drop';
import { Assignment } from 'src/app/models/assignment';
import { AssignmentService } from 'src/app/assignments/assignments.service';
import { Stage } from 'src/app/models/stage';


@Component({
  selector: 'tt-board-details',
  templateUrl: './board-details.component.html',
  styleUrls: ['./board-details.component.scss']
})
export class BoardDetailsComponent implements OnInit {

  board! : Board;
  
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
}

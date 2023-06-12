import { Component, OnInit } from '@angular/core';
import { Assignment } from 'src/app/models/assignment';
import { AssignmentService } from '../assignments.service';
import { ActivatedRoute } from '@angular/router';
import { Stage } from 'src/app/models/stage';
import { StageService } from 'src/app/stages/stage.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'tt-assignment-view-full',
  templateUrl: './assignment-view-full.component.html',
  styleUrls: ['./assignment-view-full.component.scss']
})
export class AssignmentViewFullComponent implements OnInit {

  boardId: string = "0";
  assignmentId: string = "0";
  assignment?: Assignment;
  stage?: Stage;

  constructor(private activatedRoute: ActivatedRoute,
    private assignmentService: AssignmentService,
    private stageService: StageService) { 

  }

  ngOnInit(): void {
    this.boardId = this.activatedRoute.snapshot.paramMap.get('boardId')!;
    this.assignmentId = this.activatedRoute.snapshot.paramMap.get('taskId')!;
    this.getAssignment(this.boardId, this.assignmentId);
  }

  getAssignment(boardId: string, assignmentId: string) {
    this.assignmentService.getAssignment(boardId, assignmentId)
      .subscribe((result) => { 
        this.assignment = result; 
        this.getStage(this.boardId, this.assignment.stageId);
      });
  }

  getStage(boardId: string, stageId: number) {
    this.stageService.getStage(boardId, stageId.toString())
      .subscribe((result) => this.stage = result);
  }


}

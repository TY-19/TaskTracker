import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { Assignment } from 'src/app/models/assignment';
import { AssignmentService } from '../assignment.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Stage } from 'src/app/models/stage';
import { StageService } from 'src/app/stages/stage.service';

@Component({
  selector: 'tt-assignment-view-full',
  templateUrl: './assignment-view-full.component.html',
  styleUrls: ['./assignment-view-full.component.scss']
})
export class AssignmentViewFullComponent implements OnInit {
  @Input() boardId?: string;
  @Input() assignmentId?: string;
  @Input() sidebarView: boolean = false;
  assignment?: Assignment;
  stage?: Stage;

  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private assignmentService: AssignmentService,
    private stageService: StageService) { 

  }

  ngOnInit(): void {
    this.boardId ??= this.activatedRoute.snapshot.paramMap.get('boardId')!;
    this.assignmentId ??= this.activatedRoute.snapshot.paramMap.get('taskId')!;
    this.getAssignment(this.boardId, this.assignmentId);
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['assignmentId']) {
      this.assignmentId = changes['assignmentId'].currentValue;
    }
    this.getAssignment(this.boardId ?? "0", this.assignmentId ?? "0");
  }

  getAssignment(boardId: string, assignmentId: string) {
    this.assignmentService.getAssignment(boardId, assignmentId)
      .subscribe((result) => { 
        this.assignment = result; 
        this.getStage(this.boardId ?? "0", this.assignment.stageId);
      });
  }

  getStage(boardId: string, stageId: number) {
    this.stageService.getStage(boardId, stageId.toString())
      .subscribe((result) => this.stage = result);
  }

  deleteAssignment() {
    this.assignmentService.deleteAssignment(this.boardId!, this.assignmentId!)
      .subscribe(() => {
        this.router.navigate(['/boards', this.boardId])
          .catch(error => console.log(error))
      });
  }


}

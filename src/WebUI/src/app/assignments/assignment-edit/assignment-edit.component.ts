import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AssignmentService } from '../assignments.service';
import { StageService } from 'src/app/stages/stage.service';
import { Stage } from 'src/app/models/stage';
import { Assignment } from 'src/app/models/assignment';

@Component({
  selector: 'tt-assignment-edit',
  templateUrl: './assignment-edit.component.html',
  styleUrls: ['./assignment-edit.component.scss']
})
export class AssignmentEditComponent implements OnInit {

  form!: FormGroup;

  boardId: string = "0";
  assignmentId: string = "0";

  stages: Stage[] = [];
  
  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private assignmentService: AssignmentService,
    private stageService: StageService) { 
      
    }
  
  ngOnInit(): void {
    this.boardId = this.activatedRoute.snapshot.paramMap.get('boardId')!;
    this.assignmentId = this.activatedRoute.snapshot.paramMap.get('taskId')!;
    this.initiateForm();
    this.loadAssignment();
    
  }

  initiateForm() {
    this.form = new FormGroup({
      id: new FormControl(0, [
        Validators.required
      ]),
      topic: new FormControl("", [
        Validators.required
      ]),
      description: new FormControl(),
      stage: new FormControl(),
      deadline: new FormControl(Date.now()),
      isCompleted: new FormControl(false)
    });
  }

  private loadAssignment() {
    this.assignmentService.getAssignment(this.boardId, this.assignmentId)
      .subscribe(result => {
        this.form.patchValue(result);
        this.stageService.getStages(this.boardId)
          .subscribe(stages => {
            this.stages = stages;
            let selectedStageId = this.stages.find(s => s.id == result.stageId)?.id;
            this.form.patchValue({'stage': selectedStageId});
          });
      });
  }

  onSubmit() {
    if(this.form.valid)
    {
      let assignment = {
        id: this.form.controls['id'].value,
        topic: this.form.controls['topic'].value,
        description: this.form.controls['description'].value,
        boardId: Number(this.boardId),
        stageId: this.form.controls['stage'].value,
        deadline: this.form.controls['deadline'].value,
        isCompleted: this.form.controls['isCompleted'].value,
        subparts: []
      };
      this.updateAssignment(assignment);
    }
  }

  updateAssignment(assignment: any) {
    this.assignmentService.updateAssignment(this.boardId, assignment)
      .subscribe( () => { 
          this.router.navigate(['/boards', this.boardId, 'tasks', this.assignmentId])
            .catch(error => console.log(error))
      });
  }
  
}

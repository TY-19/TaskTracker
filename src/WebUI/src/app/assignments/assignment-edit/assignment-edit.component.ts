import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AssignmentService } from '../assignment.service';
import { StageService } from 'src/app/stages/stage.service';
import { Stage } from 'src/app/models/stage';
import * as moment from 'moment';

@Component({
  selector: 'tt-assignment-edit',
  templateUrl: './assignment-edit.component.html',
  styleUrls: ['./assignment-edit.component.scss']
})
export class AssignmentEditComponent implements OnInit {
  @Input() boardId?: string;
  @Input() assignmentId?: string;
  @Input() sidebarView: boolean = false;

  form!: FormGroup;

  stages: Stage[] = [];
  mode: string = "edit";
  
  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private assignmentService: AssignmentService,
    private stageService: StageService) { 
      
    }
  
  ngOnInit(): void {
    this.setFields();
    this.initiateForm();
    this.prepareData();
  }

  private setFields() {
    this.boardId ??= this.activatedRoute.snapshot.paramMap.get('boardId')!;
    this.assignmentId ??= this.activatedRoute.snapshot.paramMap.get('taskId') ?? "0";
    this.mode = this.assignmentId == "0" ? "create" : "edit";
  }

  private initiateForm() {
    this.form = new FormGroup({
      id: new FormControl(0, [
        Validators.required
      ]),
      topic: new FormControl("", [
        Validators.required
      ]),
      description: new FormControl(),
      stage: new FormControl(null, [
        Validators.required
      ]),
      deadlineDate: new FormControl(new Date()),
      deadlineTime: new FormControl("00:00"),
      isCompleted: new FormControl(false)
    });
  }

  private prepareData() {
    if (this.mode === "edit")
      this.loadAssignment();
    else
      this.getStages();
  }

  private loadAssignment() {
    this.assignmentService.getAssignment(this.boardId!, this.assignmentId!)
      .subscribe(result => {
        this.form.patchValue(result);
        this.form.patchValue({ 'deadlineDate': result.deadline });
        this.form.patchValue({ 'deadlineTime': this.getTimeFromDateTime(result.deadline) });
        this.stageService.getStages(this.boardId!)
          .subscribe(stages => {
            this.stages = stages;
            let selectedStageId = this.stages.find(s => s.id == result.stageId)?.id;
            this.form.patchValue({'stage': selectedStageId});
          });
      });
  }

  private getStages() {
    this.stageService.getStages(this.boardId!)
      .subscribe(stages => {
        this.stages = stages;
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
        deadline: this.getDeadline(),
        isCompleted: this.form.controls['isCompleted'].value,
        subparts: []
      };
      if (this.mode === "create")
        this.createAssignment(assignment);
      else if (this.mode === "edit")
        this.updateAssignment(assignment);
    }
  }

  private getDeadline() : string {
      let deadlineDate = this.form.controls['deadlineDate'].value;
      
      let deadline : moment.Moment = moment.isMoment(deadlineDate)
        ? moment(deadlineDate)
        : moment(deadlineDate, 'YYYY-MM-DDTHH:mm:ss', true);

      let deadlineTime = this.form.controls['deadlineTime'].value;
      let timeMoment = moment(deadlineTime, 'HH:mm', true);

      deadline = moment(deadline).hour(0).minute(0)
        .add(timeMoment.hours(), 'hours')
        .add(timeMoment.minutes(), 'minutes');;

      return deadline.format('YYYY-MM-DDTHH:mm:ss')
  }

  private getTimeFromDateTime(dateTime: Date | undefined) : string {
    if (!dateTime) return "00:00";
    let momentDateTime = moment(dateTime, 'YYYY-MM-DDTHH:mm:ss');
    return momentDateTime.hours() + ":" + momentDateTime.minutes();
  }

  private createAssignment(assignment: any) {
    this.assignmentService.createAssignment(this.boardId!, assignment)
      .subscribe( () => { 
        if(!this.sidebarView) {
          this.router.navigate(['/boards', this.boardId])
            .catch(error => console.log(error))
        }
    });
  }

  private updateAssignment(assignment: any) {
    this.assignmentService.updateAssignment(this.boardId!, assignment)
      .subscribe( () => { 
        if(!this.sidebarView) {
          this.router.navigate(['/boards', this.boardId, 'tasks', this.assignmentId])
            .catch(error => console.log(error))
        }
      });
  }
  
}

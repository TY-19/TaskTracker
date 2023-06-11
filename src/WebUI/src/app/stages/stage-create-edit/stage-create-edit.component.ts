import { Component, EventEmitter, Input, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Stage } from 'src/app/models/stage';
import { StageService } from '../stage.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'tt-stage-create-edit',
  templateUrl: './stage-create-edit.component.html',
  styleUrls: ['./stage-create-edit.component.scss']
})
export class StageCreateEditComponent implements OnInit {
  @Input() stageId: number = 0;
  @Input() mode: string = "view";
  @Output() updateNotification: EventEmitter<void> = new EventEmitter<void>();
  
  form!: FormGroup;
  boardId: string = "";

  constructor(private activatedRoute: ActivatedRoute,
    private stageService: StageService) {     
  }

  ngOnInit(): void {
    this.boardId = this.activatedRoute.snapshot.paramMap.get('boardId')!;
    this.initiateForm();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['stageId']?.currentValue)
      this.loadStage(changes['stageId'].currentValue);
    else 
      this.loadStage(this.stageId);
  }

  private initiateForm()
  {
    this.form = new FormGroup({
      id: new FormControl(0),
      name: new FormControl("", [
        Validators.required,
        Validators.minLength(3),
      ])
    });
  }
  
  onSubmit() {
    if(this.form.valid)
    {
      if (!this.stageId || this.stageId == 0)
        this.createStage();
      else
        this.editStage(this.stageId);

      this.form?.reset({id: 0});
    }
  }

  createStage() {
    let toCreate = { id: 0, 
      name: this.form.controls['name'].value,
      position: 0, 
      boardId: Number(this.boardId)
    };
    this.stageService.createStage(this.boardId, toCreate)
      .subscribe(() => this.updateNotification.emit());
  }

  editStage(stageId: number) {
    let toUpdate = { id: stageId, 
      name: this.form.controls['name'].value,
      position: 0, 
      boardId: Number(this.boardId)
    }
    this.stageService.updateStage(this.boardId, toUpdate)
      .subscribe(() => this.updateNotification.emit());
  }

  loadStage(stageId: number) {
    if(this.stageId && this.stageId != 0) {        
      this.stageService.getStage(this.boardId, stageId.toString())
        .subscribe( (result) => this.form.patchValue(result))
    }
    if(this.stageId == 0)
      this.form?.reset({id: 0});
  }
}

import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { StageService } from '../stage.service';
import { ActivatedRoute } from '@angular/router';
import { DisplayModes } from 'src/app/common/enums/display-modes';
import { StageUpdateModel } from 'src/app/models/update-models/stage-update-model';

@Component({
  selector: 'tt-stage-create-edit',
  templateUrl: './stage-create-edit.component.html',
  styleUrls: ['./stage-create-edit.component.scss']
})
export class StageCreateEditComponent implements OnInit, OnChanges {
  @Input() stageId: number = 0;
  @Input() mode: DisplayModes = DisplayModes.View;
  @Output() updateNotification: EventEmitter<void> = new EventEmitter<void>();
  
  form!: FormGroup;
  boardId!: string;

  constructor(private activatedRoute: ActivatedRoute,
    private stageService: StageService) {     
  }

  ngOnInit(): void {
    this.boardId = this.activatedRoute.snapshot.paramMap.get('boardId')!;
    this.initiateForm();
  }

  ngOnChanges(changes: SimpleChanges): void {
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
        Validators.maxLength(50),
      ])
    });
  }

  private loadStage(stageId: number): void {
    if(this.stageId && this.stageId != 0) {
      this.stageService.getStage(this.boardId, stageId.toString())
        .subscribe( (result) => this.form.patchValue(result))
    }
    if(this.stageId == 0)
      this.form?.reset({id: 0});
  }
  
  onSubmit(): void {
    if(this.form.valid)
    {
      if (!this.stageId || this.stageId == 0) {
        this.createStage();
      } else {
        this.editStage(this.stageId);
      }

      this.form?.reset({id: 0});
    } else {
      this.form.markAllAsTouched();
    }
  }

  createStage(): void {
    const stage: StageUpdateModel = {
      name: this.form.controls['name'].value,
    };
    this.stageService.createStage(this.boardId, stage)
      .subscribe(() => this.updateNotification.emit());
  }

  editStage(stageId: number): void {
    const stage: StageUpdateModel = {
      name: this.form.controls['name'].value,
    }
    this.stageService.updateStage(this.boardId, stageId, stage)
      .subscribe(() => this.updateNotification.emit());
  }

  get isInEditMode(): boolean {
    return this.mode === DisplayModes.Edit;
  }
}

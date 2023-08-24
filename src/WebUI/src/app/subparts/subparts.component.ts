import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { Subpart } from '../models/subpart';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { DisplayModes } from '../common/enums/display-modes';
import { SubpartService } from './subpart.service';

@Component({
  selector: 'tt-subparts',
  templateUrl: './subparts.component.html',
  styleUrls: ['./subparts.component.scss']
})
export class SubpartsComponent implements OnInit, OnChanges {
  boardId!: string;
  @Input() assignmentId?: number;
  @Input() isUserAllowToChangeSubpartStatus: boolean = false;
  @Input() subparts?: Subpart[];
  @Input() mode: DisplayModes = DisplayModes.View;
  @Output() subpartUpdated: EventEmitter<void> = new EventEmitter<void>();

  forms: FormGroup[] = [];
  
  constructor(private subpartService: SubpartService,
    private activatedRoute: ActivatedRoute,
    private formBuilder: FormBuilder) { 

  }

  ngOnInit(): void {
    this.boardId = this.activatedRoute.snapshot.paramMap.get('id')!;
    this.initForms();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['subparts'] && !changes['subparts'].isFirstChange()) {
      this.initForms();
    }
  }

  private initForms(): void {
    if (this.subparts && this.subparts.length > 0) {
      this.forms = this.subparts.map(subpart => this.createSubpartFormGroup(subpart));
    } else {
      this.forms = [];
    }
  }

  onAddSubpart(): void {
    this.forms.push(this.createSubpartFormGroup());
  }

  onUpdateSubpartStatus(subpart: Subpart): void {
    if(!this.isUserAllowToChangeSubpartStatus) {
      return;
    }
    subpart.isCompleted = !subpart.isCompleted;
    this.subpartService
      .updateSubpart(this.boardId, this.assignmentId!, subpart.id, subpart)
      .subscribe(() => this.subpartUpdated.emit());
  }

  onRemoveSubpart(index: number): void {
    this.forms.splice(index, 1);
  }

  private createSubpartFormGroup(subpart?: Subpart): FormGroup {
    return this.formBuilder.group({
      name: [subpart?.name ?? '',
        [
          Validators.required,
          Validators.maxLength(50)
        ]],
      percentValue: [subpart?.percentValue ?? 0,
        [
          Validators.required
        ]],
      isCompleted: [subpart?.isCompleted ?? false]
    });
  }

  getSubparts(): Subpart[] {
    return this.forms.map(subpartFormGroup => ({
      id: 0,
      assignmentId: this.assignmentId ?? 0,
      ...subpartFormGroup.value
    }));
  }

  areAllSubpartsValid(): boolean {
    for (let form of this.forms) {
      if(!form.valid) {
        form.markAllAsTouched();
        return false;
      }
    }
    return true;
  }

  get isInViewMode() {
    return this.mode === DisplayModes.View;
  }
  get isInEditMode() {
    return this.mode === DisplayModes.Edit;
  }
}

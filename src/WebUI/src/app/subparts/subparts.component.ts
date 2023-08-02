import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { Subpart } from '../models/subpart';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'tt-subparts',
  templateUrl: './subparts.component.html',
  styleUrls: ['./subparts.component.scss']
})
export class SubpartsComponent implements OnInit {
  boardId!: string;
  @Input() assignmentId?: number;
  @Input() subparts?: Subpart[];
  @Input() mode: string = "details";

  forms: FormGroup[] = [];
  
  constructor(private activatedRoute: ActivatedRoute,
    private formBuilder: FormBuilder) { 

  }

  ngOnInit(): void {
    this.boardId = this.activatedRoute.snapshot.paramMap.get('boardId')!;
    this.initForms();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['subparts'] && !changes['subparts'].isFirstChange()) {
      this.initForms();
    }
  }

  private initForms(): void {
    if (this.subparts && this.subparts.length > 0) {
      this.forms = this.subparts.map((subpart) => this.createSubpartFormGroup(subpart));
    } else {
      this.forms = [];
    }
  }

  onAddSubpart() {
    this.forms.push(this.createSubpartFormGroup());
  }

  onRemoveSubpart(index: number) {
    this.forms.splice(index, 1);
  }

  createSubpartFormGroup(subpart?: Subpart): FormGroup {
    return this.formBuilder.group({
      name: [subpart?.name ?? '',
        [
          Validators.required,
          Validators.maxLength(50)
        ]],
      percentValue: [subpart?.percentValue ?? '']
    });
  }

  getSubparts(): Subpart[] {
    return this.forms.map((subpartFormGroup) => ({
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
}

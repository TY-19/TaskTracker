import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { Subpart } from '../models/subpart';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SubpartService } from './subpart.service';
import { ActivatedRoute, Router } from '@angular/router';

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
  
  constructor(private subpartService: SubpartService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
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
      this.forms.push(this.createSubpartFormGroup());
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
      name: [subpart?.name ?? '', Validators.required],
      percentValue: [subpart?.percentValue ?? '']
    });
  }

  onSubmit() {
    let allSubparts: Subpart[] = this.forms.map((subpartFormGroup) => subpartFormGroup.value);
    for (let subpart of allSubparts) {
      subpart.id = 0;
      subpart.assignmentId = this.assignmentId ?? 0;
    }
    this.subpartService
      .updateSubparts(this.boardId, this.assignmentId!, allSubparts)
      .subscribe(() => {
        this.router.navigate(['/boards', this.boardId]);
      });

  }
}

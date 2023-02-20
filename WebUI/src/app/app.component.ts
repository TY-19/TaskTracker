import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http'

@Component({
  selector: 'tt-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  test!: any;
  
  constructor(private http: HttpClient) {

  }

  ngOnInit() {
    this.http.get("/api/test").subscribe(response => this.test = response);
  }
}

import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Angular';
  users: any;

  constructor(private _http: HttpClient) {

  }


  ngOnInit(): void {
    this.getUsers();
  }

  getUsers(){
    this._http.get("https://localhost:44302/api/Users").subscribe(response => {
      this.users = response;
    });
  }

}

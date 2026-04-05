import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HeaderButtonModel } from "../../model/headerButton.model";
import { EventService } from "../../services/event.service";
import { NavService } from "../../services/nav.service";

@Component({
    selector: 'app-section-header',
    templateUrl: './section-header.component.html',
    styleUrls: ['./section-header.component.scss'],
})
export class SectionHeaderComponent implements OnInit {

    @Input()
    public title: string | undefined;

    @Input()
    public firstButton: HeaderButtonModel = new HeaderButtonModel();
    @Input()
    public secondButton: HeaderButtonModel = new HeaderButtonModel();
    @Input()
    public showInput: boolean = false;

    constructor(
        private router: Router,
        private eventService: EventService,
        private navService: NavService,
    ) {
    }

    ngOnInit(): void {
    }

}

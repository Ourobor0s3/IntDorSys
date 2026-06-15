import { Pipe, PipeTransform } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Pipe({ name: 'lang', pure: false })
export class TranslateReplacePipe implements PipeTransform {

    private variableRegEx = /\{(.+?)\}/g;

    constructor(private translate: TranslateService) {
    }

    // {{'some text'|lang:[some variables]}}
    // text example: "Text to translate to {Lang} language" - '{Lang}' - variable to replace

    transform(text: string, replaces: string[] = null) {
        if (text == null || text == undefined || text.length == 0) {
            return;
        }

        var translated = this.translate.instant(text);
        if (typeof translated !== 'string') {
            return translated;
        }
        if (translated.match(this.variableRegEx) != null && replaces != null) {
            const replaceMap: { key: string; value: string }[] = [];
            translated.match(this.variableRegEx).forEach(result => {
                replaceMap.push({ key: result, value: replaces[replaceMap.length] });
            });
            replaceMap.forEach(map => {
                translated = translated.replace(map.key, map.value);
            });
        }
        return translated;
    }
}

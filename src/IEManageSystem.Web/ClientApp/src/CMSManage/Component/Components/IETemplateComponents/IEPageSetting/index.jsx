import React from 'react';
import BaseComponentObject from '../../BaseComponents/BaseStaticComponent';
import IEPageSetting from './IEPageSetting';
import ComponentDescribe, {componentType} from '../../ComponentDescribe'

import ComponentSettingConfig from '../../BaseComponents/BaseComponent/ComponentSettingConfig';
import SettingConfig from './SettingConfig'

class ComponentObject extends BaseComponentObject{
    ComponentSettingConfigs = [
        ComponentSettingConfig.BuildPageComponentSettingConfig("PageSetting", "页面设置",
            (pageComponentSetting, setPageComponentSetting) => {
                return <SettingConfig
                    data={pageComponentSetting}
                    setData={setPageComponentSetting}
                />;
            }
        )
    ];
    Component(props){
        return <IEPageSetting {...props} />
    }
    Preview() {
        return <p>IE-页面设置</p>
    };
}

let componentDescribe = new ComponentDescribe("IEPageSetting", new ComponentObject(), componentType.other, "IE页面设置");
export default componentDescribe;
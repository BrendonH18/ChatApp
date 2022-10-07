const sort_ActiveAndObserve = (objArray) => {
    const Array = {}
    objArray.forEach(obj => {
        let key = obj["channel"]["id"]
        if(!Array[key]){
            Array[key] = {}
            Array[key].Active = []
            Array[key].Observe = []
        }
        if (obj.user.isPasswordValid) Array[key].Active.push(obj)
        Array[key].Observe.push(obj)
    });
    return Array
}

const sort_ChannelId = (objArray, availableChannels) => {

    for (let i = 0; i < availableChannels.length; i++) {
        const channel = availableChannels[i];
        if(!objArray[channel.id]){
            objArray[channel.id] = {}
            objArray[channel.id].Active = []
            objArray[channel.id].Observe = []
        }
    }
    return objArray
}

export {sort_ActiveAndObserve, sort_ChannelId}

import * as sort from "./sort"

const groupUsersByChannelId = (objArray, availableChannels) => {
	if(!objArray) return
	if(!availableChannels) return
	let Array = {}
    Array = sort.sort_ActiveAndObserve(objArray)
    Array = sort.sort_ChannelId(objArray, availableChannels)
	return Array
}

export default groupUsersByChannelId
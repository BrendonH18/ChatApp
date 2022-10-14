
import { useState, useEffect } from "react"
import { useNavigate } from "react-router-dom"
import Body from "./Components/Body"
import SearchBar from "./Components/SearchBar"



const Channels = ({ connection, channel, availableChannels, ActiveChannelID, isConnectedUsersByChannelAndStatusTruthy, connectedUsersByChannelAndStatus}) => {
    const [search, setSearch] = useState("")
    const [trimChannels, setTrimChannels] = useState(availableChannels)

    const trimAvailableChannels = () => {
        if (search === "") return setTrimChannels(availableChannels)
        setTrimChannels( availableChannels.filter(ch => ch.name.toLowerCase().includes(search.toLowerCase())))
    }

    useEffect(trimAvailableChannels,[search, availableChannels])

    useEffect(() => {
        if(!availableChannels) return
        if(!connection) return
        let id = 1;
        if(parseInt(ActiveChannelID) !== 0 || typeof ActiveChannelID !== "undefined") id = parseInt(ActiveChannelID)
        if(id === parseInt(channel.id)) return
        connection.send("JoinChannel", availableChannels.find(x=> x.id === id))
      }, [ActiveChannelID, availableChannels])
    
    return <>
    <div className="card mb-sm-3 mb-md-0 contacts_card">
        <div className="card-header">
            <SearchBar setSearch={setSearch} search={search}/>
        </div>
        <div className="card-body contacts_body">
            <Body trimChannels={trimChannels} ActiveChannelID={ActiveChannelID} isConnectedUsersByChannelAndStatusTruthy={isConnectedUsersByChannelAndStatusTruthy} connectedUsersByChannelAndStatus={connectedUsersByChannelAndStatus}/>
        </div>
        <div className="card-footer"/>
    </div>
</>
}

export default Channels
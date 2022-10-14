
import { useState, useEffect } from "react"
import { useNavigate } from "react-router-dom"
import Card from "../Formatting/Card"
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
    <Card 
        header={<SearchBar setSearch={setSearch} search={search}/>} 
        body={<Body trimChannels={trimChannels} ActiveChannelID={ActiveChannelID} isConnectedUsersByChannelAndStatusTruthy={isConnectedUsersByChannelAndStatusTruthy} connectedUsersByChannelAndStatus={connectedUsersByChannelAndStatus}/>} 
        specialFormat={{body: ["contacts_body"]}}
        />
    </>
}

export default Channels
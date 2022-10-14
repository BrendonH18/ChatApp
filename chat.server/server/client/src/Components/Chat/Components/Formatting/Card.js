import { faList } from "@fortawesome/free-solid-svg-icons"
import { useEffect, useState } from "react"

const Card = ({header, body, footer, specialFormat}) => {
    const [formats, setFormats] = useState("")
    
    const baseFormats = {
        header: ["card-header"],
        body: ["card-body"],
        footer: ["card-footer"]
    }
    
    const formatClasses = () => {
        const newBaseFormats = baseFormats
        if(!!specialFormat?.header) newBaseFormats.header.push(...specialFormat.header)
        if(!!specialFormat?.body) newBaseFormats.body.push(...specialFormat.body)
        if(!!specialFormat?.footer) newBaseFormats.footer.push(...specialFormat.footer)
        setFormats(newBaseFormats)
    }

    useEffect(() => {
        formatClasses()
    }, [specialFormat])

    // card-body contacts_body

return <>
<div className="card mb-sm-3 mb-md-0 contacts_card">
    <div className={formats.header?.join(" ")}>
        {header}
    </div>
    <div className={formats.body?.join(" ")}>
        {body}
    </div>
    <div className={formats.footer?.join(" ")}>
        {footer}
    </div>
</div>
</>
}

export default Card